using System.Text;
using FriendsGamesTools.EditorTools.BuildModes;
using UnityEngine;

namespace FriendsGamesTools
{
    public static class FGTSettingsUtils
    {
        static FGTSettings settings => SettingsInEditor<FGTSettings>.instance;
        public static bool AppleAppIdValid(StringBuilder sb = null)
        {
            if (!BuildModeSettings.instance.IOSEnabled) return true;
            if (settings.appleAppId.IsNullOrEmpty())
            {
                sb?.AppendLine("apple App Id not set");
                return false;
            }
            if (settings.appleAppId.Length != 10 || !long.TryParse(settings.appleAppId, out var _))
            {
                sb?.AppendLine("apple App Id should be 10 digits");
                return false;
            }
            return true;
        }
        static StringBuilder sb = new StringBuilder();
        public static void AppleAppIdInput() => AppleAppIdInput("Apple ID", 50);
        public static void AppleAppIdInput(string title, int labelWidth)
        {
            if (!BuildModeSettings.instance.IOSEnabled) return;
            var changed = false;
            GUILayout.BeginHorizontal();
            sb.Clear();
            var valid = AppleAppIdValid(sb);
            EditorGUIUtils.ShowValid(valid);
            var currChanged = EditorGUIUtils.TextField(title, ref settings.appleAppId, ref changed, width: 90 + labelWidth, labelWidth: labelWidth);
            GUILayout.EndHorizontal();
            if (!valid && !settings.appleAppId.IsNullOrEmpty())
                EditorGUIUtils.ColoredLabel(sb.ToString(), EditorGUIUtils.red);
            if (currChanged)
                settings.SetChanged();
        }
    }
}