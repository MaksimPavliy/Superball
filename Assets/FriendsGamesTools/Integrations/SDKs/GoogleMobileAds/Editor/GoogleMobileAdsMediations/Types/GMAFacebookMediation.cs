using System.IO;

namespace FriendsGamesTools.Integrations
{
    public class GMAFacebookMediation : GoogleMobileAdsMediation
    {
        public override string sourceURL => "https://bintray.com/google/mobile-ads-adapters-unity/GoogleMobileAdsFacebookMediation";
        public override string downloadURL => $"https://bintray.com/google/mobile-ads-adapters-unity/download_file?file_path=GoogleMobileAdsFacebookMediation%2F{version}%2FGoogleMobileAdsFacebookMediation-{version}.zip";
        public override string version => "2.8.0";
        public override bool isInProject => File.Exists("Assets/GoogleMobileAds/Editor/FacebookMediationDependencies.xml");
    }
}