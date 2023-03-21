#if MAX_SDK
using System;

namespace FriendsGamesTools.Integrations.MaxSDK
{
    [Serializable]
    public class FGTIronSourceMediationSettings : MediationSetupManager
    {
        public override Mediations type => Mediations.IRONSOURCE_NETWORK;
        public override bool GetIOSSet() => true;
        public override (bool can, string whyCant) canSetIOS => (false, "");
        public override bool GetAndroidSet() => true;
        public override (bool can, string whyCant) canSetAndroid => (false, "");
        public override string folder => "IronSource";
    }
}
#endif