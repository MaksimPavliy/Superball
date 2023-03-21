using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FriendsGamesTools.DebugTools.ChromaKey
{
    public class SceneDisablingView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI depthLabel;
        [SerializeField] TextMeshProUGUI pathLabel;
        [SerializeField] TransformView transformPrefab;
        [SerializeField] Transform viewsParent;
        [SerializeField] GameObject backParent;

#if DEBUG_TOOLS
        bool changed;
        void OnEnable()
        {
            changed = false;
            InitTransformsIfNeeded();
            curr = null;
            UpdateView();
        }
        private void OnDisable()
        {
            transformsInited = false;
        }

        #region Current transform 
        List<Transform> _roots;
        public List<Transform> roots {
            get {
                InitTransformsIfNeeded();
                return _roots;
            }
            private set => _roots = value;
        }
        HashSet<Transform> protectedTransforms = new HashSet<Transform>();
        public bool IsProtected(Transform tr) => protectedTransforms.Contains(tr);
        bool transformsInited;
        void InitTransformsIfNeeded()
        {
            if (transformsInited) return;
            transformsInited = true;
            var oldRoots = roots;
            roots = Utils.GetAllEnabledRootTransformsOnScene();
            if (oldRoots != null)
            {
                oldRoots.ForEach(root =>
                {
                    if (root == null || roots.Contains(root) || root.parent != null)
                        return;
                    roots.Add(root);
                });
            }
            UpdateProtectedTransforms();
        }
        void UpdateProtectedTransforms()
        {
            protectedTransforms.Clear();
            // Dont hide debug panel.
            ProtectTransformAndParents(transform);
            // Dont hide ability to press debug panel buttons.
            eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
            eventSystems.ForEach(e => ProtectTransformAndParents(e.transform));
            // Dont hide camera to render debug panel.
            var debugPanelCanvas = transform.GetComponentInParent<Canvas>();
            debugPanelCamera = debugPanelCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? Camera.main : debugPanelCanvas.worldCamera;
            ProtectTransformAndParents(debugPanelCamera.transform);
        }
        public EventSystem[] eventSystems { get; private set; }
        public Camera debugPanelCamera { get; private set; }
        void ProtectTransformAndParents(Transform transform)
        {
            var tr = transform;
            while (tr != null)
            {
                protectedTransforms.Add(tr);
                tr = tr.parent;
            }
        }
        Transform curr;
        static StringBuilder sb = new StringBuilder();
        public const string PathSeparator = "->";
        public static (int depth, string path) GetDepthPath(Transform curr)
        {
            int depth = 0;
            sb.Clear();
            while (curr != null)
            {
                sb.Insert(0, curr.name);
                depth++;
                curr = curr.parent;
                if (curr != null)
                    sb.Insert(0, PathSeparator);
            }
            return (depth, sb.ToString());
        }
        public static int GetDepth(Transform curr)
        {
            var (depth, _) = GetDepthPath(curr);
            return depth;
        }
        public static string GetPath(Transform curr)
        {
            var (_, path) = GetDepthPath(curr);
            return path;
        }
        #endregion

        public bool programChange;
        List<TransformView> transformViews = new List<TransformView>();
        List<Transform> transformModels = new List<Transform>();
        void UpdateView()
        {
            programChange = true;
            var (depth, path) = GetDepthPath(curr);
            depthLabel.text = depth.ToString();
            pathLabel.text = path;
            backParent.SetActive(curr != null);
            transformModels.Clear();
            if (curr == null)
                transformModels = roots.Clone();
            else
            {
                for (int i = 0; i < curr.childCount; i++)
                    transformModels.Add(curr.GetChild(i));
            }
            Utils.UpdatePrefabsList(transformViews, transformModels, transformPrefab, viewsParent, (model, view) => {
                view.Show(model, this);
            });
            programChange = false;
        }
        public void ShowTransform(Transform newCurr)
        {
            curr = newCurr;
            UpdateView();
        }
        public void OnToParentPressed() => ShowTransform(curr.parent);

        #region Enabling/disabling items
        public event Action<Transform, bool> onSetTransformEnabled;
        public void SetTransformEnabled(Transform curr, bool enabled)
        {
            if (IsProtected(curr) || curr.gameObject.activeSelf == enabled)
                return;
            curr.gameObject.SetActive(enabled);
            UpdateView();
            onSetTransformEnabled?.Invoke(curr, enabled);
        }
        #endregion
#endif
    }
}