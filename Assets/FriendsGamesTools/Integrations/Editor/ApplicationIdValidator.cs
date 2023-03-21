using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using FriendsGamesTools.EditorTools.BuildModes;

namespace FriendsGamesTools.Integrations
{
    // TODO: Merge into ApplicationIdShouldBeValid.
    public class ApplicationIdValidator
    {
        public static string androidId => PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
        public static string iosId => PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
        private static bool CommonIdValid(string id)
             => id != null
                && Regex.Matches(id, "^[0-9]+$").Count == 0; // No numbers. Android wants this.
        private static bool FGTIdValid(string id)
           => CommonIdValid(id)
                && id.StartsWith(FriendsGamesConstants.ApplicationIdPrefix) // Starts with correct company prefix.;
                && Regex.Matches(id, Regex.Escape(".")).Count == 2 // Includes only 2 dots.
                && FriendsGamesConstants.ApplicationIdPrefix.Length<id.Length; // Not empty game name.
        private static bool ExternalIdValid(string id)
            => CommonIdValid(id)
            && Regex.Matches(id, Regex.Escape(".")).Count >= 2; // Includes not less than 2 dots.
        public static bool IdValid(string id) => ProjectSettingItemManager.GetEnabled<ApplicationIdShouldBeValid>() ? FGTIdValid(id) : ExternalIdValid(id);

                
        public static string idTip = $"Should start with {FriendsGamesConstants.ApplicationIdPrefix}, include 2 dots, have no numbers, not empty game name";
        public static string idsDifferentTip = "IOS bundle id and Android package id should be the same";
        public static bool AllOk => IdValid(androidId) && androidId == iosId;
        public static void UpdateBundleIdValidationOnGUI()
        {
            var androidValid = IdValid(androidId);
            var iosValid = IdValid(iosId);

            GUILayout.Label($"androidId package name = {androidId}");
            if (!androidValid)
            {
                EditorGUIUtils.ColoredLabel("Android package name not set up correctly", Color.red);
                GUILayout.Label(idTip);
            }

            GUILayout.Label($"ios bundle id = {iosId}");
            if (!iosValid)
            {
                EditorGUIUtils.ColoredLabel("IOS bundle id not set up correctly", Color.red);
                GUILayout.Label(idTip);
            }

            if (iosId != androidId)
                EditorGUIUtils.ColoredLabel(idsDifferentTip, Color.red);

            if (AllOk)
                EditorGUIUtils.ColoredLabel("Application ids set up correctly", EditorGUIUtils.green);
        }
    }
}


