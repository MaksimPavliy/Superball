#if IAP
namespace FriendsGamesTools.IAP
{
    public class AppStoresCredentials : SettingsScriptable<AppStoresCredentials>
    {
        //public string hi;
        protected override bool inRepository => false;
        protected override bool inResources => false;

        public GooglePlayMarketCredentials googlePlayMarket = new GooglePlayMarketCredentials();
        public AppleAppStoreCredentials appleAppStore = new AppleAppStoreCredentials();
    }
}
#endif
