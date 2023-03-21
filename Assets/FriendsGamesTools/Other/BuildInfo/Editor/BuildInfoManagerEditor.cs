using System.IO;
using System.Linq;
using FriendsGamesTools.EditorTools.BuildModes;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace FriendsGamesTools
{
    public class BuildInfoManagerEditor : IPreprocessBuildWithReport
    {
        static BuildInfo settings => SettingsInEditor<BuildInfo>.instance;
        public int callbackOrder => 0;
        public static int buildNumber
        {
            get => int.Parse(PlayerSettings.iOS.buildNumber);
            set
            {
                if (float.TryParse(Application.version, out var versionFloat)) {
                    var versionInt = Mathf.RoundToInt(versionFloat * 100);
                    PlayerSettings.Android.bundleVersionCode = versionInt * 1000 + value;
                }
                PlayerSettings.iOS.buildNumber = value.ToString();
            }
        }
        public void OnPreprocessBuild(BuildReport report)
        {
            InitBuildInfoSettings();
        }
        public static void InitOnLoad() => InitBuildInfoSettings();
        public static void InitBuildInfoSettings()
        {
            BuildModesModule.DoAllReleaseChecks();
            settings.readyToRelease = string.IsNullOrEmpty(BuildModeSettings.instance.releaseErrors);
            settings.builtFrom = SystemInfo.deviceName;
            settings.buildVersion = buildNumber.ToString();
            settings.dateString = BuildInfo.CreateDateStringNow();
            (settings.commitHash, settings.branch) = TryGetCommitHashFromLocalGit();
            BuildInfoManager.Init();
        }
        private static (string commit, string branch) TryGetCommitHashFromLocalGit()
        {
            try
            {
                string NormalizeSlashes(string str) => str.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                var projectFolder = NormalizeSlashes(Application.dataPath);
                var slash = Path.DirectorySeparatorChar;
                projectFolder = projectFolder.Replace(slash + FriendsGamesManager.AssetsFolder, "");
                var head = File.ReadAllText($"{projectFolder}{slash}.git{slash}HEAD");
                const string BranchRefPrefix = "ref: ";
                string commit, branch;
                if (head.Contains(BranchRefPrefix)) {
                    head = head.Replace(BranchRefPrefix, "").Trim();
                    head = NormalizeSlashes(head);
                    commit = File.ReadAllText($"{projectFolder}{slash}.git{slash}{head}");
                    var refParts = head.Split(Path.DirectorySeparatorChar);
                    branch = refParts.Last();
                } else
                {
                    commit = head;
                    branch = "detached_head";
                }
                commit = commit.Trim();
                //Debug.Log($"({commit})");
                return (commit, branch);
            }
            catch {
                return default;
            }
        }
    }
}