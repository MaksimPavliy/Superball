using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.Integrations.MaxSDK
{
    public class MaxSDKSettings : SettingsScriptable<MaxSDKSettings>
    {
        protected override string SubFolder => "MaxSDK";
        public string SDKKey = FriendsGamesConstants.MAXSDKKey;
        public bool customSDKKeyAllowed = false;
        public bool interstitialsEnabled = true;
        public bool rewardedVideosEnabled = true;
        public bool bannerAdsEnabled = true;
        public ConscentAppliementOption conscentInEditor = ConscentAppliementOption.Applied;
        public ConscentAppliementOption conscentInDevBuild = ConscentAppliementOption.RealValueFromMax;
        public ConscentAppliementOption conscent
        {
            get
            {
                if (Application.isEditor)
                    return conscentInEditor;
                if (BuildMode.release)
                    return ConscentAppliementOption.RealValueFromMax;
                return conscentInDevBuild;
            }
        }

        [Serializable] public class PlatformSettings
        {
            public bool enabled;
            public string rewardedAdUnitId;
            public string interstitialAdUnitId;
            public string bannerAdUnitId;
        }
        public PlatformSettings ios, android;
        public PlatformSettings currPlatform => TargetPlatformUtils.current == TargetPlatform.Android ? android : ios;

        public List<Mediations> enabledMediations = new List<Mediations> {
            Mediations.ADMOB_NETWORK,  Mediations.UNITY_NETWORK,
            Mediations.FACEBOOK_MEDIATE, Mediations.IRONSOURCE_NETWORK, Mediations.TAPJOY_NETWORK
        };
    }

    public enum ConscentAppliementOption { RealValueFromMax, Applied, NotApplied }

    public enum Mediations
    {
        // Names should be the same as pluginData.MediatedNetworks[i].Name.
        // Implemented.
        ADMOB_NETWORK,
        FACEBOOK_MEDIATE,
        UNITY_NETWORK,
        APPLOVIN_NETWORK,
        ADCOLONY_NETWORK,
        TAPJOY_NETWORK,
        IRONSOURCE_NETWORK,
        INMOBI_NETWORK,
        // Not implemented.
        AMAZON_NETWORK,
        CHARTBOOST_NETWORK,
        FYBER_NETWORK,
        GOOGLEADMANAGER_NETWORK,
        MAIO_NETWORK,
        MINTEGRAL_NETWORK,
        MYTARGET_NETWORK,
        NEND_NETWORK,
        OGURY_PRESAGE_NETWORK,
        SMAATO_NETWORK,
        TENCENT_NETWORK,
        TIKTOK_NETWORK,
        VERIZON_NETWORK,
        VOODOOADS_NETWORK,
        VUNGLE_NETWORK,
        YANDEX_NETWORK
    }
}