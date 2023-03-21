#if ECSGame
using FriendsGamesTools.ECSGame;
#endif
using System;
using FriendsGamesTools.EditorTools.BuildModes;
using UnityEngine;

namespace FriendsGamesTools
{
    public class BuildInfo : SettingsScriptable<BuildInfo>
    {
        protected override bool inRepository => false;
        public string branch;
        public string commitHash;
        public string buildVersion;
        public string builtFrom;
        public string cloudBuildTargetName;
        public bool isCloud;
        public string cloudBuildNumber;
        public bool readyToRelease;
        public string dateString;

        public string platform => Application.isEditor ? "Editor" : Application.platform.ToString();

        public string commitHashShort => commitHash != null ? commitHash.WithMaxLength(10) : string.Empty;
        string dataVersion
#if ECSGame
            => DataVersion.versionInd.ToString();
#else
            => "0";
#endif
        string readyToReleaseString => readyToRelease ? "RTR" : "NFR";
        public static string CreateDateStringNow() => $"{DateTime.Now.Day.ToString("00")}.{DateTime.Now.Month.ToString("00")}.{DateTime.Now.Year.ToString().Substring(2)}";
        public override string ToString()
            => $"{Application.version}({buildVersion})-{dataVersion}-" +
            $"{BuildModeSettings.mode}-{readyToReleaseString}-{platform}-{commitHashShort}-{builtFrom}-{dateString}";
    }
}