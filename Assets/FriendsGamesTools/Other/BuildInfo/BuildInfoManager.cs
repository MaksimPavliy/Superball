using FriendsGamesTools.EditorTools.BuildModes;
using MiniJSON;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools
{
    public static class BuildInfoManager
    {
        static TextAsset cloudManifest;
        static Dictionary<string, object> manifestDict;
        static BuildInfo settings => BuildInfo.instance;
        static BuildInfoManager() => Init();
        public static void Init()
        {
            cloudManifest = Resources.Load("UnityCloudBuildManifest.json") as TextAsset;
            settings.isCloud = cloudManifest != null;
            if (settings.isCloud)
            {
                manifestDict = Json.Deserialize(cloudManifest.text) as Dictionary<string, object>;
                settings.commitHash = GetStringFromManifest("scmCommitId");
                settings.branch = GetStringFromManifest("scmBranch");
                settings.cloudBuildNumber = GetStringFromManifest("buildNumber");
                settings.builtFrom = $"Cloud#{settings.cloudBuildNumber}";
                settings.cloudBuildTargetName = GetStringFromManifest("cloudBuildTargetName");
                if (settings.buildVersion == "0")
                    settings.buildVersion = settings.cloudBuildNumber;
            }
            else
            {
                settings.cloudBuildNumber = "-1";
                settings.cloudBuildTargetName = "";
            }
            buildInfo = settings.ToString();

            if (BuildModeSettings.release && !settings.readyToRelease)
                Debug.LogError("Release check errors exist:\n" + BuildModeSettings.instance.releaseErrors);
        }
        static string GetStringFromManifest(string key)
        {
            if (manifestDict != null && manifestDict.TryGetValue(key, out var valueObj))
                return valueObj?.ToString() ?? string.Empty;
            else
                return string.Empty;
        }
        public static string buildInfo { get; private set; }
    }
}