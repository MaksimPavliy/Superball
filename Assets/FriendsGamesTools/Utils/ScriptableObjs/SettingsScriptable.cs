using UnityEngine;

namespace FriendsGamesTools
{
    // In editor, ot get or create settings, use SettingsScriptableCreator.GetSettingsInstance().
    // At runtime, access settings using         SettingsScriptable.LoadFromResources()
    public abstract class SettingsScriptable<TSelf> : ScriptableObject
        where TSelf : SettingsScriptable<TSelf>
    {
        protected virtual string SubFolder => null;
        private string GeneratedFolderName
            =>
            (inFGTGeneratedFolder ? FriendsGamesManager.GeneratedFolder : FriendsGamesManager.AssetsFolder)
            + (inRepository ? "" : $"/LocalSettings")
            + (SubFolder != null ? $"/{SubFolder}" : "");
        protected virtual string ParentFolder
            => GeneratedFolderName + (inResources ? "/Resources" : "");
        public virtual string AssetName => GetType().Name;
        private string AssetNameWithExtension => $"{AssetName}.asset";
        public string AssetPath => $"{ParentFolder}/{AssetNameWithExtension}";
        protected virtual bool inResources => true;
        protected virtual bool canUseDefault => true;
        protected virtual bool inRepository => true;
        protected virtual bool inFGTGeneratedFolder => true;
        private string ResoucesPath => inResources ? AssetName : null;
        public static TSelf LoadFromResources()
        {
            var inst = CreateInstance<TSelf>();
            Debug.Assert(inst.inResources || inst.canUseDefault, "For settings not in build but in editor, use SettingsCreator.GetSettingsInstance()");
            var loadedInst = Resources.Load<TSelf>(inst.ResoucesPath);
            if (loadedInst != null)
                return loadedInst;
#if UNITY_EDITOR
            // When assets are being updated, Resources.Load does not work, so overwriting data with default data can occur.
            // It should be prevented.
            if (UnityEditor.EditorApplication.isUpdating)
                return null;
#endif
            if (inst.canUseDefault) 
                return inst;
            return null;
        }
        static TSelf _instance;
        public static TSelf instance => _instance ?? (_instance = LoadFromResources());

#if UNITY_EDITOR
        public void SaveInEditorPlayMode()
        {
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
    public static class SettingsScriptableUtils
    {
#if UNITY_EDITOR
        public static void SetChanged<T>(this SettingsScriptable<T> settings)
            where T : SettingsScriptable<T>
            => UnityEditor.EditorUtility.SetDirty(settings);
#endif
        public static void SetChanged(this Transform tr)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(tr);
#endif
        }
    }
}
