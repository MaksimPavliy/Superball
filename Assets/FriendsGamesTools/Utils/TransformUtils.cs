using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public static partial class Utils
    {
        public static bool Contains(this Collider collider, Vector3 pt, float eps = 0.001f)
            => Vector3.SqrMagnitude(pt - collider.ClosestPoint(pt)) < eps * eps;
        public static Bounds GetBoundsWithChildren(this Transform transform) {
            var bounds = new Bounds(transform.position, Vector3.zero);
            transform.GetComponentsInChildren<Renderer>().ForEach(r => bounds.Encapsulate(r.bounds));
            return bounds;
        }
        public static T GetOrAddTransform<T>(this GameObject go) where T : Component
        {
            var t = go.GetComponent<T>();
            if (t == null)
                t = go.AddComponent<T>();
            return t;
        }
        public static bool DestroyIfPlaying(this MonoBehaviour mono) {
            if (!Application.isPlaying) return false;
            MonoBehaviour.Destroy(mono);
            return true;
        }
        public static Transform GetChild(this Transform transform, string name) {
            for (int i = 0; i < transform.childCount; i++) {
                if (transform.GetChild(i).name == name)
                    return transform.GetChild(i);
            }
            return null;
        }
        public static void Safe<T>(this T component, Action action)
            where T : Component
        {
            if (component != null && action != null)
                action();
        }
        public static void SetTextSafe(this TextMeshProUGUI text, string str) {
            if (text != null)
                text.text = str;
        }
        public static async Task MovingTo(this Transform tr, Vector3 tgtPos, float duration, bool realtime = false, bool local = false)
        {
            var startPos = local ? tr.localPosition : tr.position;
            await AsyncUtils.SecondsWithProgress(duration, progress => {
                var currPos = Vector3.Lerp(startPos, tgtPos, progress);
                if (local)
                    tr.localPosition = currPos;
                else
                    tr.position = currPos;
            }, realtime);
        }
        public static async Task ScalingTo(this Transform tr, float tgtScale, float duration, bool realTime = false)
        {
            var startScale = tr.localScale.x;
            await AsyncUtils.SecondsWithProgress(duration, progress => tr.localScale = Vector3.one * Mathf.Lerp(startScale, tgtScale, progress), realTime);
        }
        public static List<Transform> GetAllEnabledRootTransformsOnScene()
            => UnityEngine.Object.FindObjectsOfType<Transform>().Filter(tr => tr.parent == null && tr.gameObject.name != "New Game Object");
        public static List<T> FindSceneObjectsWithInactive<T>()
            where T : Behaviour
        {
            var objectsInScene = new List<T>();
            IterateSceneObjectsWithInactive<T>(t => objectsInScene.Add(t));
            return objectsInScene;
        }
        public static void IterateSceneObjectsWithInactive<T>(Action<T> action) where T : Behaviour
        {
            var all = Resources.FindObjectsOfTypeAll(typeof(T)) as T[];
            foreach (T t in all)
            {
#if UNITY_EDITOR
                if (UnityEditor.EditorUtility.IsPersistent(t.transform.root.gameObject))
                    continue;
#endif
                if (!(t.gameObject.hideFlags == HideFlags.NotEditable || t.gameObject.hideFlags == HideFlags.HideAndDontSave))
                    action?.Invoke(t);
            }
        }
        public static void SetDefaultLocalPosition(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var t = go.GetComponent<T>();
            if (t == null)
                t = go.AddComponent<T>();
            return t;
        }
        public static void IterateInterfacesInScene<T>(Action<T> action) where T : class
            => SceneManager.GetActiveScene().GetRootGameObjects().ForEach(root => root.GetInterfacesInChildren<T>().ForEach(action));
        public static List<T> GetInterfacesInChildren<T>(this GameObject go) where T : class
            => go.transform.GetInterfacesInChildren<T>();
        public static List<T> GetInterfacesInChildren<T>(this Transform t, bool includeInactive = false) where T : class
            => t.GetComponentsInChildren<Component>(includeInactive).ConvertAll(c => c as T).Filter(c => c != null);
        public static void IterateChildren(this Transform tr, Action<Transform> action)
        {
            action?.Invoke(tr);
            for (int i = 0; i < tr.childCount; i++)
                IterateChildren(tr.GetChild(i), action);
        }
        public static void IterateChildren(this GameObject go, Action<GameObject> action)
        {
            action?.Invoke(go);
            for (int i = 0; i < go.transform.childCount; i++)
                IterateChildren(go.transform.GetChild(i).gameObject, action);
        }
        public static void SetLayerRecursively(this GameObject go, int layer)
            => go.transform.IterateChildren(tr => tr.gameObject.layer = layer);
        public static void DestroyAlways(this Transform tr)
        {
            if (Application.isPlaying)
                MonoBehaviour.Destroy(tr.gameObject);
            else
                MonoBehaviour.DestroyImmediate(tr.gameObject);
        }
        public static void DestroyChildren(this Transform tr)
        {
            for (int i = tr.childCount-1; i >= 0; i--)
                tr.GetChild(i).DestroyAlways();
        }
        public static void DestroyChildrenImmediate(this Transform tr)
        {
            for (int i = tr.childCount - 1; i >= 0; i--)
                MonoBehaviour.DestroyImmediate(tr.GetChild(i).gameObject);
        }
        public static void SetActiveSafe(this GameObject go, bool val)
        {
            if (go != null)
                go.SetActive(val);
        }
        public static void SetActiveSafe(this Component c, bool val)
        {
            if (c != null)
                c.gameObject.SetActive(val);
        }
        public static void SetSpriteSafe(this Image pic, Sprite sprite)
        {
            if (pic != null)
                pic.sprite = sprite;
        }
        public static bool IsInsideScreen(Vector3 pos, Camera cam = null)
        {
            if (cam == null)
                cam = Camera.main;
            var viewportPos = cam.WorldToViewportPoint(pos);
            return viewportPos.x.InRange(0, 1) && viewportPos.y.InRange(0, 1);
        }
        static StringBuilder sb;
        public static string FullName(this Transform transform)
        {
            if (sb == null) sb = new StringBuilder();
            sb.Clear();
            do
            {
                sb.Insert(0, transform.name);
                transform = transform.parent;
                if (transform == null)
                    break;
                sb.Insert(0, ".");
            } while (true);
            return sb.ToString();
        }
        public static PositionRotation GetPositionRotation(this Transform tr)
            => new PositionRotation { position = tr.position, rotation = tr.rotation };
        public static void SetPositionRotation(this Transform tr, PositionRotation posRot)
            => tr.SetPositionAndRotation(posRot.position, posRot.rotation);
        public static PositionRotation GetLocalPositionRotation(this Transform tr)
            => new PositionRotation { position = tr.localPosition, rotation = tr.localRotation };
        public static void SetLocalPositionRotation(this Transform tr, PositionRotation posRot) {
            tr.localPosition = posRot.position;
            tr.localRotation = posRot.rotation;
        }
    }
    public struct PositionRotation
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}