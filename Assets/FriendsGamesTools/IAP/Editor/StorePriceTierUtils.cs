#if IAP
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.IAP
{
    public static class StorePriceTierUtils
    {
        public static void Test()
        {
            Debug.Assert(priceTiersConsumable[28 - 1] == 27.99m);
            Debug.Assert(priceTiersConsumable[80 - 1] == 399.99m);
            Debug.Assert(priceTiersSubscription[130 - 1] == 99.99m);
            Debug.Assert(priceTiersSubscription[170 - 1] == 199.99m);
            Debug.Assert(priceTiersSubscription[193 - 1] == 399.99m);
        }
        // All prices cap with android's 400$ cap. https://support.google.com/googleplay/android-developer/table/3541286
        // https://appstoreconnect.apple.com/WebObjects/iTunesConnect.woa/ra/ng/app/1495023292/pricingMatrix/consumable
        public static List<decimal> priceTiersConsumable = new List<decimal> {
            0.99m, 1.99m, 2.99m, 3.99m, 4.99m, 5.99m, 6.99m, 7.99m,  8.99m, 9.99m, 10.99m, 11.99m, 12.99m, 13.99m, 14.99m,
            15.99m, 16.99m, 17.99m, 18.99m, 19.99m, 20.99m, 21.99m, 22.99m, 23.99m, 24.99m, 25.99m, 26.99m, 27.99m, 28.99m,
            29.99m, 30.99m, 31.99m, 32.99m, 33.99m, 34.99m, 35.99m, 36.99m, 37.99m, 38.99m, 39.99m, 40.99m, 41.99m, 42.99m,
            43.99m, 44.99m, 45.99m, 46.99m, 47.99m, 48.99m, 49.99m, 54.99m, 59.99m, 64.99m, 69.99m, 74.99m, 79.99m, 84.99m,
            89.99m, 94.99m, 99.99m, 109.99m, 119.99m, 124.99m, 129.99m, 139.99m, 149.99m, 159.99m, 169.99m, 174.99m, 179.99m,
            189.99m, 199.99m, 209.99m, 219.99m, 229.99m, 239.99m, 249.99m, 299.99m, 349.99m, 399.99m
        };
        // https://appstoreconnect.apple.com/WebObjects/iTunesConnect.woa/ra/ng/app/1495023292/pricingMatrix/nonConsumable
        public static List<decimal> priceTiersNonConsumable => priceTiersConsumable;
        // https://appstoreconnect.apple.com/WebObjects/iTunesConnect.woa/ra/ng/app/1495023292/pricingMatrix/recurring
        public static List<decimal> priceTiersSubscription = new List<decimal> {
            0.49m, 0.99m, 1.49m, 1.99m, 2.49m, 2.99m, 3.49m, 3.99m, 4.49m, 4.99m, 5.49m, 5.99m, 6.49m, 6.99m,
            7.49m, 7.99m, 8.49m, 8.99m, 9.49m, 9.99m, 10.49m, 10.99m, 11.49m, 11.99m, 12.49m, 12.99m, 13.49m,
            13.99m, 14.49m, 14.99m, 15.49m, 15.99m, 16.49m, 16.99m, 17.49m, 17.99m, 18.49m, 18.99m, 19.49m,
            19.99m, 20.49m, 20.99m, 21.49m, 21.99m, 22.49m, 22.99m, 23.49m, 23.99m, 24.49m, 24.99m, 25.49m,
            25.99m, 26.49m, 26.99m, 27.49m, 27.99m, 28.49m, 28.99m, 29.49m, 29.99m, 30.99m, 31.99m, 32.99m,
            33.99m, 34.99m, 35.99m, 36.99m, 37.99m, 38.99m, 39.99m, 40.99m, 41.99m, 42.99m, 43.99m, 44.99m,
            45.99m, 46.99m, 47.99m, 48.99m, 49.99m, 50.99m, 51.99m, 52.99m, 53.99m, 54.99m, 55.99m, 56.99m,
            57.99m, 58.99m, 59.99m, 60.99m, 61.99m, 62.99m, 63.99m, 64.99m, 65.99m, 66.99m, 67.99m, 68.99m,
            69.99m, 70.99m, 71.99m, 72.99m, 73.99m, 74.99m, 75.99m, 76.99m, 77.99m, 78.99m, 79.99m, 80.99m,
            81.99m, 82.99m, 83.99m, 84.99m, 85.99m, 86.99m, 87.99m, 88.99m, 89.99m, 90.99m, 91.99m, 92.99m,
            93.99m, 94.99m, 95.99m, 96.99m, 97.99m, 98.99m, 99.99m, 100.99m, 101.99m, 102.99m, 103.99m, 104.99m,
            105.99m, 106.99m, 107.99m, 108.99m, 109.99m, 110.99m, 111.99m, 112.99m, 113.99m, 114.99m, 115.99m,
            116.99m, 117.99m, 118.99m, 119.99m, 120.99m, 121.99m, 122.99m, 123.99m, 124.99m, 129.99m, 134.99m,
            139.99m, 144.99m, 149.99m, 154.99m, 159.99m, 164.99m, 169.99m, 174.99m, 179.99m, 184.99m, 189.99m,
            194.99m, 199.99m, 204.99m, 209.99m, 214.99m, 219.99m, 224.99m, 229.99m, 234.99m, 239.99m, 244.99m,
            249.99m, 254.99m, 259.99m, 264.99m, 269.99m, 274.99m, 279.99m, 284.99m, 289.99m, 294.99m, 299.99m,
            329.99m, 349.99m, 399.99m
        };
        static List<decimal> GetPriceTiers(ProductType type)
            => type == ProductType.Consumable ? priceTiersConsumable :
                (type == ProductType.NonConsumable ? priceTiersNonConsumable : priceTiersSubscription);
        public static int ToApplePriceTier(decimal price, ProductType type)
        {
            var tierInd = ToApplePriceTierInd(price, type);
            if (tierInd == -1)
                return -1;
            var tier = tierInd + 1;
            return tier;
        }
        public static int ToApplePriceTierInd(decimal price, ProductType type)
            => GetPriceTiers(type).IndexOf(price);
        public static decimal ToApplePrice(int tier, ProductType type)
        {
            var tiers = GetPriceTiers(type);
            var tierInd = tier - 1;
            if (!tiers.IndIsValid(tierInd))
                return -1;
            return tiers[tierInd];
        }

        public static string[] GetPriceTitles(ProductType type)
        {
            InitIfNeeded();
            return type == ProductType.Consumable ? priceTitlesConsumable :
                (type == ProductType.NonConsumable ? priceTitlesNonConsumable : priceTitlesSubscription);
        }
        static string[] priceTitlesConsumable, priceTitlesNonConsumable, priceTitlesSubscription;
        static void InitIfNeeded()
        {
            if (priceTitlesConsumable != null)
                return;
            priceTitlesConsumable = priceTiersConsumable.ConvertAll(p => p.ToString()).ToArray();
            priceTitlesNonConsumable = priceTiersNonConsumable.ConvertAll(p => p.ToString()).ToArray();
            priceTitlesSubscription = priceTiersSubscription.ConvertAll(p => p.ToString()).ToArray();
        }
    }
}
#endif