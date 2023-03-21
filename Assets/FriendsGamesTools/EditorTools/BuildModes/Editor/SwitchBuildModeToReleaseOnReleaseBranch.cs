using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class SwitchBuildModeToReleaseOnReleaseBranch : IPreprocessBuildWithReport
    {
        public static void InitOnLoad() => Do();
        public int callbackOrder => 1;
        public void OnPreprocessBuild(BuildReport report) => Do();
        static void Do()
        {
            if (BuildInfo.instance.branch == "release" && BuildModeSettings.mode != BuildModeType.Release)
                BuildModesModule.SetMode(BuildModeType.Release);
        }
    }
}