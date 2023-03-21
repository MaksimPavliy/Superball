#if MAX_SDK
using System;

namespace FriendsGamesTools.Integrations.MaxSDK
{
    [Serializable]
    public class FBMediationSettings : MediationSetupManager
    {
        public override Mediations type => Mediations.FACEBOOK_MEDIATE;
        public override bool GetIOSSet() => FBSetupManager.ExistsAndConfigured;
        public override (bool can, string whyCant) canSetIOS => (false, "setup FB to allow ads from it");

        MaxSDKAndroidAllowClearText _androidSecurity;
        MaxSDKAndroidAllowClearText androidSecurity 
            => _androidSecurity ?? (_androidSecurity = new MaxSDKAndroidAllowClearText(MaxSDKAndroidAllowClearText.Type.FB));
        public override bool GetAndroidSet()
            => FBSetupManager.ExistsAndConfigured && androidSecurity.GetCompleted();
        public override void SetupAndroid() => androidSecurity.DoSetup();
        public override (bool can, string whyCant) canSetAndroid => (FBSetupManager.ExistsAndConfigured, "setup FB to allow ads from it");
        public override string folder => "Facebook";
    }
}
#endif