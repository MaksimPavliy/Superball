using TMPro;
using UnityEditor;
using UnityEngine;
#if UI
using TMPro;
#endif

namespace FriendsGamesTools
{
    public abstract class ExampleAsset<T> where T : Object
    {
        public T asset { get; private set; }
        string notAvailableExplanation;
        protected abstract TypeFilterName type { get; }
        private string TypeFilterName => type.ToString();
        public ExampleAsset(string fileName, string notAvailableExplanation = "")
        {
            var okIfNotFound = !string.IsNullOrEmpty(notAvailableExplanation);
            asset = AssetByTypeAndName.Find<T>(fileName, type, okIfNotFound);
            this.notAvailableExplanation = notAvailableExplanation;
        }
        public void ShowOnGUI(string title, string desc = null)
        {
            //GUILayout.BeginHorizontal();
            ShowLabel(title);
            if (asset != null)
                EditorGUILayout.ObjectField(asset, typeof(T), false);
            else if (!string.IsNullOrEmpty(notAvailableExplanation))
                EditorGUILayout.LabelField(notAvailableExplanation);
            if (!string.IsNullOrEmpty(desc))
                ShowLabel(desc);
            GUILayout.Space(10);
            //GUILayout.EndHorizontal();
        }
        void ShowLabel(string label)
        {
            if (label.Contains("<") || label.Contains("\n"))
                EditorGUIUtils.RichMultilineLabel(label);
            else
                GUILayout.Label(label);
        }
    }
    public class ExamplePrefab : ExampleAsset<GameObject>
    {
        public ExamplePrefab(string fileName, string notAvailableExplanation = null)
            : base(fileName, notAvailableExplanation) { }
        protected override TypeFilterName type => TypeFilterName.Prefab;
    }
    public class ExampleScript : ExampleAsset<MonoScript>
    {
        public ExampleScript(string fileName, string notAvailableExplanation = null) 
            : base(fileName, notAvailableExplanation) { }
        protected override TypeFilterName type => TypeFilterName.MonoScript;
    }
    public class ExampleFolder : ExampleAsset<Object>
    {
        public ExampleFolder(string fileName, string notAvailableExplanation = null)
            : base(fileName, notAvailableExplanation) { }
        protected override TypeFilterName type => TypeFilterName.Folder;
    }
    public class ExampleFontAsset : ExampleAsset<TMP_FontAsset>
    {
        public ExampleFontAsset(string fileName, string notAvailableExplanation = null)
            : base(fileName, notAvailableExplanation) { }
        protected override TypeFilterName type => TypeFilterName.TMP_FontAsset;
    }
    public class ExampleFontTTF : ExampleAsset<Font>
    {
        public ExampleFontTTF(string fileName, string notAvailableExplanation = null)
            : base(fileName, notAvailableExplanation) { }
        protected override TypeFilterName type => TypeFilterName.Font;
    }
    public class ExampleMaterial : ExampleAsset<Material>
    {
        public ExampleMaterial(string fileName, string notAvailableExplanation = null)
            : base(fileName, notAvailableExplanation) { }
        protected override TypeFilterName type => TypeFilterName.Material;
    }
}


