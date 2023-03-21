#if DEBUG_CONFIG
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    [ExecuteAlways]
    public class ConfigExportImport : MonoBehaviour
    {
#if DEBUG_CONFIG
        #region Main functionality
        private static string path => Application.persistentDataPath + "/SaveBalance.dat";
        static List<BalanceSettings> GetBalanceSettings()
        {
            var balanceSettings = BalanceSettings.instances;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                balanceSettings = Utils.FindSceneObjectsWithInactive<BalanceSettings>();
#endif
#if ECS_LOCATIONS
            balanceSettings = balanceSettings.Filter(b => !(b is ECSGame.Locations.ILocationBalanceSettings));
#endif
            balanceSettings.SortBy(b => b.GetType().Name);
            return balanceSettings;
        }
        string Export()
        {
            var sb = new StringBuilder();
            var balanceSettings = GetBalanceSettings();
            foreach (var balance in balanceSettings)
                sb.Append($"{balance.GetType().Name}:\n{JsonUtility.ToJson(balance)}\n\n");
            return sb.ToString();
        }
        public static int GetConfigsCount(string text) => !string.IsNullOrEmpty(text) ? text.Count("\n\n") : -1;
        public static void Import(string text) {
            if (string.IsNullOrEmpty(text))
                return;
            text = text.ToLf();
            if (!text.EndsWith("\n\n"))
                text += "\n\n";
            var balanceSettings = GetBalanceSettings();
            foreach (var configInstance in balanceSettings)
            {
                var endInd = text.IndexOf("\n\n");
                var startInd = text.IndexOf(":\n") + 2;
                var json = text.Substring(startInd, endInd - startInd + 1);
                text = text.Remove(0, endInd + 2);
                //Debug.Log(json);
                if (configInstance == null)
                    continue; 
                JsonUtility.FromJsonOverwrite(json, configInstance);
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    UnityEditor.EditorUtility.SetDirty(configInstance);
#endif
                ConfigDebugView.instance?.UpdateConfigValuesView();
            }

        }
        public static string TryReadFile()
        {
            if (!File.Exists(path))
                return null;
            return File.ReadAllText(path);
        }
#endregion

#region Actions
        public void Save() => File.WriteAllText(path, Export());
        public static void Load() => Import(TryReadFile());
        public void Copy() => Export().CopyToClipboard();
        public void Paste() => Import(StringUtils.PasteFromClipboard());
        public static void ClearSave() => File.Delete(path);
        public static bool saveExists => File.Exists(path);
#endregion

#region Input
        public void OnSavePressed() => Save();
        public void OnLoadPressed() => Load();
        public void OnCopyPressed() => Copy();
        public void OnClearSavePressed() => ClearSave();
        public void OnApplyFromClipboardPressed() => Paste();
        public bool save, load, copy, paste, clear;
        private void Update()
        {
            if (Application.isPlaying)
                return;
            if (save)
            {
                save = false;
                Save();
            }
            if (load)
            {
                load = false;
                Load();
            }
            if (copy)
            {
                copy = false;
                Copy();
            }
            if (paste)
            {
                paste = false;
                Paste();
            }
            if (clear)
            {
                clear = false;
                ClearSave();
            }
        }
        #endregion
#endif
    }
}
#endif