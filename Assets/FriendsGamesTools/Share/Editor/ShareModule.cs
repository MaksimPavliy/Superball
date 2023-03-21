using System;
using System.Text;
using FriendsGamesTools.EditorTools.BuildModes;
using UnityEngine;

namespace FriendsGamesTools.Share
{
    public class ShareModule : RootModule
    {
        public const string define = "SHARE";
        public override string Define => define;
        public override HowToModule HowTo() => new ShareModule_HowTo();
        protected override string debugViewPath => "Share/Debug/ShareDebugView";

#if SHARE
        ShareSettings settings => SettingsInEditor<ShareSettings>.instance;
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            var changed = false;
            if (BuildModeSettings.instance.IOSEnabled)
            {
                GUILayout.BeginHorizontal();
                EditorGUIUtils.ShowValid(GameNameIOSValid());
                EditorGUIUtils.TextField("ios game name", ref settings.gameNameIOS, ref changed);
                GUILayout.EndHorizontal();
                FGTSettingsUtils.AppleAppIdInput();
            }

            if (changed)
                settings.SetChanged();
        }
        StringBuilder sb = new StringBuilder();
        public override string DoReleaseChecks()
        {
            sb.Clear();
            GameNameIOSValid(sb);
            FGTSettingsUtils.AppleAppIdValid(sb);
            return sb.ToString();
        }
        protected bool GameNameIOSValid(StringBuilder sb = null)
        {
            if (!BuildModeSettings.instance.IOSEnabled)
                return true;
            if (settings.gameNameIOS.IsNullOrEmpty())
            {
                sb?.AppendLine("ios game name not set");
                return false;
            }
            return true;
        }
#endif
    }
}