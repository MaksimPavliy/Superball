#if MAX_SDK
using System;

namespace FriendsGamesTools.Integrations.MaxSDK
{
    [Serializable]
    public class AdColonyMediationSettings : MediationSetupManager
    {
        public override Mediations type => Mediations.ADCOLONY_NETWORK;
        public override bool GetIOSSet() => true;
        public override (bool can, string whyCant) canSetIOS => (false, "");
        public override bool GetAndroidSet() => androidSecurity.GetCompleted();
        public override (bool can, string whyCant) canSetAndroid => (true, "");
        public override void SetupAndroid() => androidSecurity.DoSetup();

        MaxSDKAndroidAllowClearText _androidSecurity;
        MaxSDKAndroidAllowClearText androidSecurity
            => _androidSecurity ?? (_androidSecurity = new MaxSDKAndroidAllowClearText(MaxSDKAndroidAllowClearText.Type.AdColony));
        public override string folder => "AdColony";
    }
}
#endif