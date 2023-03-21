using System;
using System.Collections;
using System.Collections.Generic;

namespace FriendsGamesTools.IAP
{
    // Available at runtime.
    public class IAPSettings : SettingsScriptable<IAPSettings>
    {
        public bool logs = true;
        public bool customTeamId = false;
        public string appleSKU, appleTeamID;

        public List<ConsumableProductSettings> consumables = new List<ConsumableProductSettings>();
        public List<NonConsumableProductSettings> nonConsumables = new List<NonConsumableProductSettings>();
        public SubscriptionSettings subscription = new SubscriptionSettings();
        public bool validatePurchases = true;
        public bool IOSRestorePurchasesButtonExists = false;

        public List<string> deletedProductIds = new List<string>();

        #region Mocking purchases
        public Mode defaultModeInEditor = Mode.Mocked;
        public Mode defaultModeInBuild = Mode.Real;
        public DebugRestorePurchasesMode debugMode = DebugRestorePurchasesMode.TestRestorePurchasesButton;
        public float debugSubscriptionRemainingOnAppLaunch;
#if IAP
        public List<string> debugProductIdsToReceive
            => allProducts.Filter(p => p.debugModePurchased && p.active).ConvertAll(p => p.productId);
#endif
        #endregion

#if IAP
        public ProductsEnumerator allProducts { get; private set; } = new ProductsEnumerator();
        public AbstractProductSettings GetProduct(string productId) => allProducts.Find(p => p.productId == productId);
        public void OnBeforeExport() => subscription.OnBeforeExport();

        #region Unique product ids
        public void OnProductIdDeleted(string productId)
        {
            if (!deletedProductIds.Contains(productId))
                deletedProductIds.Add(productId);
        }
        public (bool, string) CheckProductId(string productId)
        {
            bool ok = true;
            string error = string.Empty;
            allProducts.ForEachWithInd((p1, i1) => {
                if (p1.productId != productId)
                    return;
                allProducts.ForEachWithInd((p2, i2) => {
                    if (i2 <= i1)
                        return;
                    if (p1.productId == p2.productId)
                    {
                        ok = false;
                        error = $"duplicate product id {p1.productId}";
                    }
                });
                if (deletedProductIds.Contains(productId))
                {
                    ok = false;
                    error = $"reusing deleted product id {p1.productId}";
                }
            });
            return (ok, error);
        }
        #endregion
#endif
    }
    public enum Mode { Real, Mocked }
    public enum DebugRestorePurchasesMode { TestRestorePurchasesButton, TestTransactionsOnInit }
    public class ProductsEnumerator : IEnumerable<AbstractProductSettings>
    {
        IAPSettings config => IAPSettings.instance;
        public IEnumerator<AbstractProductSettings> GetEnumerator()
        {
            foreach (var p in config.consumables)
                yield return p;
            foreach (var p in config.nonConsumables)
                yield return p;
            foreach (var p in config.subscription.products)
                yield return p;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    public enum ProductType { Consumable, NonConsumable, Subscription }
}