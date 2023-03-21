using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class EnableMultiDex : ProjectSettingItem
    {
        public override string name => "ENABLE_MULTIDEX";
        public override string description => "Multidex support should be enabled"; // otherwise android will crash when lots of lib in it.
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (settings.AndroidEnabled && !GetCompleted())
                sb.AppendLine("Multidex is not set");
        }
        public override bool canSetup => true;
        public override void SetupPlatform(BuildTargetGroup platform)
        {
            if (settings.AndroidEnabled && !GetCompleted())
                SetupAndroid();
        }

        const string defaultPath = "Assets/Plugins/Android/mainTemplate.gradle";
        const string defaultContentsPath = FriendsGamesManager.MainPluginFolder + "/EditorTools/BuildModes/Editor/Multidex/defaultMainTemplate.gradle.txt";
        static string defaultContents => AssetDatabase.LoadAssetAtPath<TextAsset>(defaultContentsPath).text;
        const string searchedString = "multiDexEnabled true";
        const int minTargetSDK = 21;
        public static bool GetCompleted()
        {
            if ((int)PlayerSettings.Android.minSdkVersion < minTargetSDK)
                return false;
            if (!File.Exists(defaultPath))
                return false;
            var text = File.ReadAllText(defaultPath);
            return text.Contains(searchedString);
        }

        static void SetupAndroid()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(defaultPath));
            File.WriteAllText(defaultPath, defaultContents);
            PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)minTargetSDK;
        }
    }
}
