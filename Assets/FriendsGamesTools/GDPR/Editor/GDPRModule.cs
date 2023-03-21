using System.Collections.Generic;
using System.Text;
using FriendsGamesTools.EditorTools.BuildModes;
using FriendsGamesTools.UI;
using UnityEngine;

namespace FriendsGamesTools
{
    public class GDPRModule : RootModule
    {
        public const string define = "GDPR";
        public override string Define => define;
        public override HowToModule HowTo() => new GDPRModule_HowTo();
        public override List<string> dependFromModules => base.dependFromModules.Adding(WindowsModule.define);

#if GDPR
        GDPRSettings settings => SettingsInEditor<GDPRSettings>.instance;
        BuildModeSettings buildModeSettings => SettingsInEditor<BuildModeSettings>.instance;
        protected override void OnCompiledGUI()
        {
            var changed = false;

            if (buildModeSettings.IOSEnabled)
            {
                EditorGUIUtils.RichLabel("iOS", TextAnchor.MiddleLeft, fontStyle: FontStyle.Bold);
                GUILayout.BeginHorizontal();
                EditorGUIUtils.ShowValid(IsIOSTermsOfUseValid());
                EditorGUIUtils.TextField("terms of use URL", ref settings.iOSTermsOfUseURL, ref changed);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUIUtils.ShowValid(IsIOSPrivacyPolicyValid());
                EditorGUIUtils.TextField("privacy policy URL", ref settings.iOSPrivacyPolicyURL, ref changed);
                GUILayout.EndHorizontal();
            }

            if (buildModeSettings.AndroidEnabled)
            {
                EditorGUIUtils.RichLabel("Android", TextAnchor.MiddleLeft, fontStyle: FontStyle.Bold);
                GUILayout.BeginHorizontal();
                EditorGUIUtils.ShowValid(IsAndroidTermsOfUseValid());
                EditorGUIUtils.TextField("terms of use URL", ref settings.androidTermsOfUseURL, ref changed);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUIUtils.ShowValid(IsAndroidPrivacyPolicyValid());
                EditorGUIUtils.TextField("privacy policy URL", ref settings.androidPrivacyPolicyURL, ref changed);
                GUILayout.EndHorizontal();
            }

            WindowEditorUtils.ShowWindow<GDPRWindow, GDPRWindowPrefabSettings>(settings.window, ref changed, true);

            var state = GDPRManager.state;
            if (EditorGUIUtils.Toolbar("in editor", ref state, ref changed))
                GDPRManager.state = state;

            var openMain = GDPRSettings.instance.openMainMenuOnClose;
            EditorGUIUtils.Toggle("open MainMenu onClose", ref openMain, ref changed);
            GDPRSettings.instance.openMainMenuOnClose = openMain;

            if (changed)
                settings.SetChanged();
        }
        bool IsStringValid(string str, string errorDesc, StringBuilder sb)
        {
            if (str.IsNullOrEmpty())
            {
                sb?.AppendLine(errorDesc);
                return false;
            }
            return true;
        }
        bool IsIOSTermsOfUseValid(StringBuilder sb = null) => IsStringValid(settings.iOSTermsOfUseURL, "terms of use should be set", sb);
        bool IsIOSPrivacyPolicyValid(StringBuilder sb = null) => IsStringValid(settings.iOSPrivacyPolicyURL, "privacy policy should be set", sb);
        bool IsAndroidTermsOfUseValid(StringBuilder sb = null) => IsStringValid(settings.androidTermsOfUseURL, "terms of use should be set", sb);
        bool IsAndroidPrivacyPolicyValid(StringBuilder sb = null) => IsStringValid(settings.androidPrivacyPolicyURL, "privacy policy should be set", sb);
        StringBuilder sb = new StringBuilder();
        public override string DoReleaseChecks()
        {
            sb.Clear();

            if (buildModeSettings.IOSEnabled)
            {
                IsIOSTermsOfUseValid(sb);
                IsIOSPrivacyPolicyValid(sb);
            }

            if (buildModeSettings.AndroidEnabled)
            {
                IsAndroidTermsOfUseValid(sb);
                IsAndroidPrivacyPolicyValid(sb);
            }

            return sb.ToString();
        }
#endif
    }
}