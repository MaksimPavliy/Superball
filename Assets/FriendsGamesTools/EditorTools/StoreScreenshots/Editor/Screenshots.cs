#if SCREENSHOTS
using FriendsGamesTools.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.Screenshots
{
    [Serializable]
    public class ScreenDesc
    {
        public string name;
        public string desc;
        public Vector2Int resolution;
        public ScreenMargins margins;
    }
    public class Screenshots
    {
        static ScreenshotsSettings settings => SettingsInEditor<ScreenshotsSettings>.instance;
        static float lastScreenTime;
        public static void OnEnable()
            => SettingsInEditor<ScreenSettings>.EnsureExists();
        public static void Update()
        {
            if (!settings.autoScreenshotsEnabled)
                return;
            if (lastScreenTime + settings.autoTakeScreenshotDelay <= Time.realtimeSinceStartup)
                TakeNextScreenshot();
        }
        public static void OnGUI()
        {
            var changed = false;
            GUILayout.BeginHorizontal();
            EditorGUIUtils.PushEnabling(Application.isPlaying);
            if (GUILayout.Button("Screenshot now"))
                TakeNextScreenshot();
            EditorGUIUtils.PopEnabling();
            settings.autoScreenshotsEnabled = GUILayout.Toggle(settings.autoScreenshotsEnabled, "take screens automatically");
            settings.autoTakeScreenshotDelay = EditorGUILayout.FloatField("delay", settings.autoTakeScreenshotDelay);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("After each screen, device is changed to next");
            GUILayout.Label("Selected devices:");
            EditorGUIUtils.Popup("", ref settings.selected, ref changed);
            if (GUILayout.Button("<"))
                ShiftScreen(-1);
            GUILayout.Label(settings.screens[screenInd].name);
            if (GUILayout.Button(">"))
                ShiftScreen(1);
            if (GUILayout.Button("Edit devices in inspector..."))
                Selection.activeObject = settings;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"Open folder {fullFolderPath}"))
                EditorUtility.RevealInFinder(folder);
            if (GUILayout.Button("Clear folder"))
                Directory.Delete(folder, true);
            GUILayout.EndHorizontal();

            if (changed)
                EditorUtils.SetDirty(settings);
        }
        static Camera _camera;
        static Camera camera { get {
                if (_camera == null)
                    _camera = Camera.main;
                return _camera;
            }
        }
        static int screenInd;
        static void TakeNextScreenshot()
        {
            lastScreenTime = Time.realtimeSinceStartup;
            TakeScreenshot(settings.screens[screenInd]);
            ShiftScreen(1);
        }
        static void ShiftScreen(int shift)
            => screenInd = (screenInd + shift + settings.screens.Count) % settings.screens.Count;
        public const string folder = "Screenshots";
        public static string fullFolderPath 
            => $"{Application.dataPath.Replace("/Assets", "")}/{Screenshots.folder}/";
        static async void TakeScreenshot(ScreenDesc screen) {
            scripts = Resources.FindObjectsOfTypeAll<MonoBehaviour>().ToList()
                .ConvertAll(f => f as IHasScreenSizeChangeCallback).Filter(f => f != null);
            var currentRes = Screen.currentResolution;
            ScreenSettings.instance.marginsInEditor = screen.margins;
            SetResolution(screen.resolution.x, screen.resolution.y);
            var cameras = UnityEngine.Object.FindObjectsOfType<Camera>().Filter(c => c.targetTexture == null);
            var rt = RenderTexture.GetTemporary(screen.resolution.x, screen.resolution.y, 24);
            var screenShot = new Texture2D(screen.resolution.x, screen.resolution.y, TextureFormat.RGB24, false);
            var cameraForUI = Enter2CameraMode();
            cameras.Add(cameraForUI);

            await TakeScreenshotFromCamera(rt, cameras);

            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, screen.resolution.x, screen.resolution.y), 0, 0); RenderTexture.active = null; // JC: added to avoid errors
            RenderTexture.ReleaseTemporary(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            Exit2CameraMode(cameraForUI);

            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);
            string filename = $"{fullFolderPath}/{screen.resolution.x}x{screen.resolution.y} {DateTime.Now.ToString("dd-MM HH-mm-ss")}.png";
            File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));

            SetResolution(currentRes.width, currentRes.height);
            await Awaiters.EndOfFrame;
            SendResolutionCallback();
        }
        static async Task TakeScreenshotFromCamera(RenderTexture rt, List<Camera> cameras)
        {
            cameras = cameras.Filter(c => c != null);
            cameras.SortBy(c => c.depth);

            cameras.ForEach(c=> {
                c.targetTexture = rt;
                c.Render();
            });
            await Awaiters.EndOfFrame;
            SendResolutionCallback();
            await Awaiters.EndOfFrame;
            SendResolutionCallback();
            cameras.ForEach(c =>
            {
                c.Render();
                c.targetTexture = null;
            });
        }
        static void SetResolution(int width, int height)
        {
            Screen.SetResolution(width, height, FullScreenMode.Windowed);
            SendResolutionCallback();
        }
        static void SendResolutionCallback() => scripts.ForEach(s => s.OnScreenSizeChanged());
        static List<IHasScreenSizeChangeCallback> scripts;

#region 2 camera mode
        // Allows to render screenshot without applying posteffects on UI
        // Screenshots are rendered using camera, so overlay UI wont be on screenshot.
        // So I remove UI from first camera and add it to second in camera space.
        static List<Canvas> overlayCanvases;
        static int UILayer;
        static Camera Enter2CameraMode()
        {
            // Get overlay canvases.
            var canvases = Transform.FindObjectsOfType<Canvas>().ToList();
            overlayCanvases = canvases.Filter(c => c.renderMode == RenderMode.ScreenSpaceOverlay);
            if (overlayCanvases.Count == 0)
                return null;

            // Create camera.
            var uiCam = MonoBehaviour.Instantiate(camera);
            uiCam.transform.parent = camera.transform;
            const int FarAwayDist = 1000;
            uiCam.transform.localPosition = Vector3.one * FarAwayDist;
            uiCam.transform.localRotation = Quaternion.identity;
            uiCam.transform.localScale = Vector3.one;
            uiCam.GetComponents<Behaviour>().ForEach(m =>
            {
                if (m is MonoBehaviour || m is AudioListener)
                    MonoBehaviour.Destroy(m);
            });
            uiCam.clearFlags = CameraClearFlags.Nothing;
            uiCam.depth = 100; // Big depth.
            MonoBehaviour.Destroy(uiCam.GetComponent<AudioListener>());

            // Render camera on top and move UI to it.
            overlayCanvases.ForEach(c=> {
                c.renderMode = RenderMode.ScreenSpaceCamera;
                c.worldCamera = uiCam;
                const float eps = 0.01f;
                c.planeDistance = uiCam.nearClipPlane + eps;
            });
            UILayer = 1 << LayerMask.NameToLayer("UI");
            uiCam.cullingMask = UILayer;
            uiCam.depth++;

            return uiCam;
        }
        static void Exit2CameraMode(Camera uiCamera)
        {
            if (uiCamera == null)
                return;

            // Return UI to prev camera.
            overlayCanvases.ForEach(c =>
            {
                c.renderMode = RenderMode.ScreenSpaceOverlay;
            });

            // Destroy camera.
            MonoBehaviour.Destroy(uiCamera.gameObject);
        }
#endregion
    }
}
#endif