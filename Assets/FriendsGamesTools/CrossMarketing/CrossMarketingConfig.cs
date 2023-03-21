using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools
{
    [Serializable]
    public class CrossMarketingAppData
    {
        public int id = -1;
        public string appName;
        public const string noId = "???????";
        public string appleID = noId;
        public string androidPackageId = noId;
        public Sprite icon = null;
        public bool showIOS = true;
        public bool showAndroid = true;

#if CROSS_MARKETING
        public static bool IdIsValid(string id) => !id.IsNullOrEmpty() && !id.Equals(noId);
        public bool available => platformAvailable && icon != null;
        private bool platformAvailable => TargetPlatformUtils.current == TargetPlatform.IOS ? availableIOS : availableAndroid;
        private bool availableIOS => showIOS && IdIsValid(appleID) && appleID != FGTSettings.instance.appleAppId;
        private bool availableAndroid => showAndroid && IdIsValid(androidPackageId) && androidPackageId != Application.identifier;
#endif
    }
    public class CrossMarketingConfig : ScriptableObject
    {
#if CROSS_MARKETING
        static CrossMarketingConfig _instance;
        public static CrossMarketingConfig instance => _instance ??
            (_instance = Resources.Load<CrossMarketingConfig>("CrossMarketingConfig"));

#endif
        public List<CrossMarketingAppData> data;
    }
}