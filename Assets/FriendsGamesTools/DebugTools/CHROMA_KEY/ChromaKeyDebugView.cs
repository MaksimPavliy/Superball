using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools.ChromaKey
{
    public class ChromaKeyDebugView : FGTModuleDebugPanel
    {
        public override string tab => "ChromaKey";
        public override string module => "CHROMA_KEY";
        public override bool wholeTab => true;
        [SerializeField] Toggle enabledCheckbox;
        [SerializeField] GameObject enabledParent;
        [SerializeField] SceneDisablingView sceneDisabling;

#if CHROMA_KEY
        new ChromaKeySettings settings => ChromaKeySettings.instance;
        bool changed;
        protected override void Awake()
        {
            base.Awake();
            sceneDisabling.onSetTransformEnabled += SetTransformEnabled;
            InitCameras();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!Application.isPlaying) return;
            changed = false;
            if (!settings.enabled)
                InitCameras();
            if (settings.enabled)
                ApplyDisablingSettings();
            UpdateEnabledView();
        }
        private void OnDisable()
        {
            SaveInEditorPlayMode();
        }
        void SaveInEditorPlayMode()
        {
#if UNITY_EDITOR
            if (changed)
                settings.SaveInEditorPlayMode();
#endif
        }

        #region Enabled
        private void UpdateEnabledView()
        {
            sceneDisabling.programChange = true;
            enabledCheckbox.isOn = settings.enabled;
            SetEnabledForCameras(settings.enabled);
            enabledParent.SetActive(settings.enabled);
            sceneDisabling.programChange = false;
        }
        public void OnEnabledChanged()
        {
            if (sceneDisabling.programChange)
                return;
            sceneDisabling.programChange = true;
            settings.enabled = enabledCheckbox.isOn;
            ApplyDisablingSettings(!settings.enabled);
            UpdateEnabledView();
            sceneDisabling.programChange = false;
        }
        void ApplyDisablingSettings(bool reverse = false)
        {
            var s = new string[] { SceneDisablingView.PathSeparator };
            var allSettings = settings.disabledNames.ConvertAll<string, string[]>(path => path.Split(s, StringSplitOptions.None));
            sceneDisabling.roots.ForEach(r => ApplyDisablingSettings(0, allSettings, r, reverse));
        }
        void ApplyDisablingSettings(int depth, List<string[]> settings, Transform tr, bool reverse)
        {
            var disableCurr = false;
            List<string[]> childSettings = null;
            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i].Length <= depth || settings[i][depth] != tr.name)
                    continue;
                if (settings[i].Length == depth + 1)
                    disableCurr = true;
                else
                {
                    if (childSettings == null)
                        childSettings = new List<string[]>();
                    childSettings.Add(settings[i]);
                }
            }
            if (disableCurr)
                tr.gameObject.SetActive(reverse);
            if (childSettings == null || childSettings.Count == 0)
                return;
            for (int i = 0; i < tr.childCount; i++)
                ApplyDisablingSettings(depth + 1, childSettings, tr.GetChild(i), reverse);
        }
        #endregion

        #region Cameras
        List<CameraData> cameras = new List<CameraData>();
        class CameraData
        {
            public Camera camera;
            public CameraClearFlags flags;
            public Color color;
        }
        private void SetEnabledForCameras(bool enabled)
        {
            cameras.ForEach(c => {
                if (enabled)
                {
                    c.camera.backgroundColor = Color.green;
                    c.camera.clearFlags = CameraClearFlags.Color;
                } else
                {
                    c.camera.backgroundColor = c.color;
                    c.camera.clearFlags = c.flags;
                }
            });
        }
        void InitCameras()
        {
            var uiLayer = LayerMask.GetMask("UI");
            cameras = FindObjectsOfType<Camera>().Filter(c =>
            {
                return c.clearFlags != CameraClearFlags.Nothing && c.cullingMask != uiLayer;
            }).ConvertAll(c =>
                  new CameraData { camera = c, color = c.backgroundColor, flags = c.clearFlags });
        }
        #endregion
        
        #region Enabling/disabling items
        void SetTransformEnabled(Transform curr, bool enabled)
        {
            var path = SceneDisablingView.GetPath(curr);
            var disabledInSettings = settings.disabledNames.Contains(path);
            var disabled = !enabled;
            if (disabled == disabledInSettings)
                return;
            if (disabled)
                settings.disabledNames.Add(path);
            else
                settings.disabledNames.Remove(path);
            UpdateEnabledView();
        }
        #endregion
#endif
    }
}