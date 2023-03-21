using System.Collections.Generic;

namespace FriendsGamesTools.Integrations
{
    public class GoogleMobileAdsTestSuiteSetupManager : LibrarySetupManager<GoogleMobileAdsTestSuiteSetupManager>
    {
        public const string define = "GOOGLE_MOBILE_ADS_TEST_SUITE";
        public override string Define => define;
        public override string SomeClassNameWithNamespace => "GoogleMobileAdsMediationTestSuite.Api.MediationTestSuite";
        public override string parentModule => GoogleMobileAdsSetupManager.define;
        public override List<string> dependFromModules
            => base.dependFromModules.Adding(GoogleMobileAdsSetupManager.define);
#if !GOOGLE_MOBILE_ADS_TEST_SUITE
        public override bool configured => false;
#else
        public override bool configured => true;
#endif
        protected override string SDKDownloadURL => "https://dl.google.com/googleadmobadssdk/GoogleMobileAdsMediationTestSuite.unitypackage";
        protected override bool canCheckUpdate => true;
        protected override bool unzipAfterDownload => false;

#if GOOGLE_MOBILE_ADS_TEST_SUITE
#endif
    }
}