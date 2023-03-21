using System.Text;
using UnityEditor.Advertisements;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public class GMAUnityAdsMediation : GoogleMobileAdsMediation
    {
        public override string sourceURL => "https://bintray.com/google/mobile-ads-adapters-unity/GoogleMobileAdsUnityAdsMediation#files/GoogleMobileAdsUnityAdsMediation";
        public override string downloadURL => $"https://bintray.com/google/mobile-ads-adapters-unity/download_file?file_path=GoogleMobileAdsUnityAdsMediation%2F{version}%2FGoogleMobileAdsUnityAdsMediation-{version}.zip";
        public override string version => "2.4.2";
        public override string SomeClassNameWithNamespace => "GoogleMobileAds.Common.Mediation.UnityAds.IUnityAdsClient";

        public override bool Valid(StringBuilder sb = null)
        {
            if (!base.Valid(sb))
                return false;

            if (AdvertisementSettings.enabled)
            {
                sb?.AppendLine("Unity's native ads service should be turned off");
                return false;
            }
            return true;
        }
        protected override void ShowDetails(ref bool changed)
        {
            base.ShowDetails(ref changed);
            if (AdvertisementSettings.enabled && GUILayout.Button("turn off unity ads native service"))
                AdvertisementSettings.enabled = false;
        }
    }
}