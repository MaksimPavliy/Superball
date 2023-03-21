using System;
using UnityEngine;

namespace FriendsGamesTools.IAP
{
    [Serializable]
    public abstract class AbstractProductSettings
    {
        public virtual string GetProductIdSuffix() => productIdSuffix;
        public string productId => GetProductId(GetProductIdSuffix());
        public string title = "", description = "";
        public const string StubScreenshotPath = FriendsGamesManager.MainPluginFolder + "/IAP/View/StubIAPScreenshot.png";
        public string appleReviewScreenshotPath = StubScreenshotPath;
        public float price;
        public bool active = true;
        public string productIdSuffix = "";
        public bool debugModePurchased;
        public static string GetProductId(string productIdSuffix) => $"{Application.identifier}.{productIdSuffix}";
        public static string GetProductSuffix(string productId) => productId.Substring(Application.identifier.Length + 1);
        [NonSerialized] public bool showProductDetails;
        public bool removesInterstitials;
    }
}