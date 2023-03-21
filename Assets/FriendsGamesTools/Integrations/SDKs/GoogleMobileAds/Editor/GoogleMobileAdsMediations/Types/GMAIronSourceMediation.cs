using FriendsGamesTools.EditorTools.BuildModes;
using System.IO;
using System.Text;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public class GMAIronSourceMediation : GoogleMobileAdsMediation
    {
        public override string sourceURL => "https://bintray.com/google/mobile-ads-adapters-unity/GoogleMobileAdsIronSourceMediation";
        public override string downloadURL => $"https://bintray.com/google/mobile-ads-adapters-unity/download_file?file_path=GoogleMobileAdsIronSourceMediation%2F{version}%2FGoogleMobileAdsIronSourceMediation-{version}.zip";
        public override string version => "1.8.0";
        public override string SomeClassNameWithNamespace => "GoogleMobileAds.Common.Mediation.IronSource.IIronSourceClient";
        public override bool Valid(StringBuilder sb = null) => base.Valid(sb) && AndroidValid();
        protected override void ShowDetails(ref bool changed)
        {
            base.ShowDetails(ref changed);
            if (BuildModeSettings.instance.AndroidEnabled && !AndroidValid() && GUILayout.Button("setup android"))
                SetupAndroid();
        }

        #region Android
        const string defaultProGuardFilePath 
            = "Assets/FriendsGamesTools/Integrations/SDKs/GoogleMobileAds" +
            "/Editor/GoogleMobileAdsMediations/Types/proguard-user - for GMAIronSourceMediation.txt";
        const string ProGuardFilePath = "Assets/Plugins/Android/proguard-user.txt";
        string _proguardContents;
        bool proguardContentsLoaded;
        string proguardContents => proguardContentsLoaded ? _proguardContents :
            (File.Exists(ProGuardFilePath)?File.ReadAllText(ProGuardFilePath):"").ToLf();
        bool AndroidValid(StringBuilder sb = null)
        {
            if (!BuildModeSettings.instance.AndroidEnabled)
                return true;
            if (string.IsNullOrEmpty(proguardContents))
            {
                sb?.AppendLine("proguard file for iron source not set");
                return false;
            }
            return true;
        }
        void SetupAndroid()
        {
            var text = File.ReadAllText(defaultProGuardFilePath);
            File.WriteAllText(ProGuardFilePath, text);
        }
        #endregion
    }
}