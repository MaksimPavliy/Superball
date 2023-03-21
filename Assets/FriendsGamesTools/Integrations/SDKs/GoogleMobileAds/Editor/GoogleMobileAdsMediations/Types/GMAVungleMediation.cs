namespace FriendsGamesTools.Integrations
{
    public class GMAVungleMediation : GoogleMobileAdsMediation
    {
        public override string sourceURL => "https://bintray.com/google/mobile-ads-adapters-unity/GoogleMobileAdsVungleMediation";
        public override string downloadURL => $"https://bintray.com/google/mobile-ads-adapters-unity/download_file?file_path=GoogleMobileAdsVungleMediation%2F{version}%2FGoogleMobileAdsVungleMediation-{version}.zip";
        public override string version => "3.3.0";
        public override string SomeClassNameWithNamespace => "GoogleMobileAds.Common.Mediation.Vungle.IVungleClient";
    }
}