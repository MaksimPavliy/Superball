using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.ModulesUpdates
{
    public class UpdatesConfig : ScriptableObject
    {
        static string path => $"{FriendsGamesManager.MainPluginFolder}/Manager/Editor/Updates/UpdatesConfig.asset";
        static UpdatesConfig _instance;
        public static UpdatesConfig instance => _instance ?? (_instance = AssetDatabase.LoadAssetAtPath<UpdatesConfig>(path));
        public List<ModuleChange> changes = new List<ModuleChange>();
    }
    [Serializable]
    public class ModuleChange
    {
        public string AffectedModule = "";
        public string whatsChanged = "";
        public string upgradeGuide = "";
        public string guid = "";
    }
}
