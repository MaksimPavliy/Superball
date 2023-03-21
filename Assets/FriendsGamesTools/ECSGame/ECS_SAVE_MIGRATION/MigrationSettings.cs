using FriendsGamesTools.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public class MigrationSettings : SettingsScriptable<MigrationSettings>
    {
        [Serializable]
        public class VersionMetaData
        {
            public int version = -1;
            public string metaData = "";
            public string shownVersion = "";
            public string whatsNew = "";
            public bool noWhatsNewOk = false;
            public string testSave = "";
            public bool noTestSaveOk = false;
            public string dontForget = "";
        }
        [CollectionElementTitle("version")]
        public List<VersionMetaData> metaDatas = new List<VersionMetaData>();
        public int debugShowMigrationFrom = -1;

#if ECS_SAVE_MIGRATION
        public VersionMetaData Get(int version) => metaDatas.Find(m => m.version == version);
        public VersionMetaData currVersion => Get(DataVersion.versionInd);
        public WorldMetaData GetMetaData(int version, bool canBeNull = false)
        {
            var savedMetaData = Get(version);
            if (savedMetaData == null)
            {
                if (!canBeNull)
                    Debug.LogError($"no metadata found for verison {version}");
                return null;
            }
            return WorldMetaData.DecodeFromString(savedMetaData.metaData);
        }

        public WhatsNewPrefab newVersionWindow = new WhatsNewPrefab();
        public bool noNewVersionWindowOk;
#endif
    }
    [Serializable]
    public class WhatsNewPrefab : WindowPrefabSettings<NewVersionWindow>
    {
        public override string title => "What's new window";
        public override string defaultPath
            => $"{FriendsGamesManager.MainPluginFolder}/ECSGame/ECS_SAVE_MIGRATION/NewVersionWindow.prefab";
    }
}