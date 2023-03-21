using FriendsGamesTools.EditorTools.BuildModes;
using System.Text;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public class GMAAppLovinMediation : GoogleMobileAdsMediation
    {
        public override string sourceURL => "https://bintray.com/google/mobile-ads-adapters-unity/GoogleMobileAdsAppLovinMediation";
        public override string downloadURL => $"https://bintray.com/google/mobile-ads-adapters-unity/download_file?file_path=GoogleMobileAdsAppLovinMediation%2F{version}%2FGoogleMobileAdsAppLovinMediation-{version}.zip";
        public override string version => "4.5.0";
        public override string SomeClassNameWithNamespace => "GoogleMobileAds.Common.Mediation.AppLovin.IAppLovinClient";

        public override bool Valid(StringBuilder sb = null)
        {
            if (!base.Valid(sb))
                return false;

            if (string.IsNullOrEmpty(settings.applovinAPIKey))
            {
                sb?.AppendLine("applovinAPIKey not set for applovin mediation");
                return false;
            }

            var valid = true;
            if (!AndroidValid(sb))
                valid = false;
            if (!IOSValid(sb))
                valid = false;
            return valid;
        }

        protected override void ShowDetails(ref bool changed)
        {
            base.ShowDetails(ref changed);
            EditorGUIUtils.TextField("applovinAPIKey", ref settings.applovinAPIKey, ref changed);
            if (BuildModeSettings.instance.AndroidEnabled && !AndroidValid() && GUILayout.Button("setup android"))
                SetupManifest();
            if (BuildModeSettings.instance.IOSEnabled && !IOSValid() && GUILayout.Button("setup ios"))
                SetupPostProcessor();
        }

        #region Android
        bool AndroidValid(StringBuilder sb = null) => !BuildModeSettings.instance.AndroidEnabled || IsManifestOK(sb);
        AndroidManifestManager manifest = new AndroidManifestManager("Assets/Plugins/Android/GoogleMobileAdsAppLovinMediation/AndroidManifest.xml");
        bool IsManifestOK(StringBuilder sb = null)
        {
            var key = manifest.GetParam("android:value", "manifest", "application", "meta-data");
            if (key != settings.applovinAPIKey)
            {
                sb?.AppendLine("applovin mediation not set for android");
                return false;
            }
            return true;
        }
        void SetupManifest()
        {
            manifest.ReplaceParam(settings.applovinAPIKey, "android:value", "manifest", "application", "meta-data");
            manifest.Save();
        }
        #endregion

        #region IOS
        bool IOSValid(StringBuilder sb = null) => !BuildModeSettings.instance.IOSEnabled || IsPostprocessorOK(sb);
        ScriptChangeManager postprocessor = new ScriptChangeManager("Assets/GoogleMobileAds/Editor/AppLovinPostProcessBuild.cs");
        bool IsPostprocessorOK(StringBuilder sb = null)
        {
            if (!postprocessor.text.Contains(settings.applovinAPIKey))
            {
                sb?.AppendLine("applovin mediation not set for ios");
                return false;
            }
            return true;
        }
        void SetupPostProcessor()
        {
            var lines = postprocessor.text.Split('\n');
            var lineInd = lines.FindIndex(l=>l.Contains("\"AppLovinSdkKey\""))+1;
            lines[lineInd] = $"                               \"{settings.applovinAPIKey}\");";
            postprocessor.text = string.Join("\n", lines);
            postprocessor.Save();
        }
        #endregion
    }
}