#if IAP
using FriendsGamesTools.EditorTools.BuildModes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.IAP
{
    public abstract class AbstractProductSettingsEditor<TSettings>
        where TSettings : AbstractProductSettings, new()
    {
        protected IAPSettings config => SettingsInEditor<IAPSettings>.instance;
        protected abstract List<TSettings> products { get; }
        protected abstract string listName { get; }
        TSettings toRemove;
        bool ios => BuildModeSettings.instance.IOSEnabled;
        bool android => BuildModeSettings.instance.AndroidEnabled;
        public void ShowProductsList(ref bool changed)
        {
            var currChanged = changed;
            var count = products.Count();
            // Header.
            EditorGUIUtils.InHorizontal(() =>
            {
                EditorGUIUtils.ShowValid(products.All(p => ProductValid(p)));
                if (count > 0)
                    GUILayout.Label($"{listName}({count}):");
                else
                    GUILayout.Label($"{listName} list empty");
                if (GUILayout.Button($"add {listName}"))
                {
                    var product = new TSettings();
                    product.showProductDetails = true;
                    products.Add(product);
                    currChanged = true;
                }
            });
            // Items.
            foreach (var p in products)
                ShowProduct(p, ref currChanged);
            if (toRemove != null)
            {
                products.Remove(toRemove);
                toRemove = null;
            }
            changed = currChanged;
        }
        StringBuilder sb = new StringBuilder();
        void ShowProduct(TSettings p, ref bool changed)
        {
            var type = p.GetProductTypeEditMode();
            var currChanged = changed;
            sb.Clear();
            var valid = ProductValid(p, sb);
            EditorGUIUtils.InHorizontal(() =>
            {
                EditorGUIUtils.Indent();
                EditorGUIUtils.ShowValid(valid);
                EditorGUIUtils.ShowOpenClose(ref p.showProductDetails);
                GUILayout.Label(p.productId);
                var priceTierInd = StorePriceTierUtils.ToApplePriceTierInd((decimal)p.price, type);
                if (EditorGUIUtils.Popup(ref priceTierInd, StorePriceTierUtils.GetPriceTitles(type), ref currChanged, 100))
                    p.price = (float)StorePriceTierUtils.ToApplePrice(priceTierInd + 1, type);
                ValidateScreenshot(p, ref currChanged);
                if (EditorGUIUtils.XButton())
                    toRemove = p;
            });
            if (p.showProductDetails)
            {
                EditorGUIUtils.TextField("product id suffix", ref p.productIdSuffix, ref changed);
                ShowCertainProduct(p, ref currChanged);
                ShowTitleDescription(ref p.title, ref p.description, ref currChanged);
                EditorGUIUtils.Toggle(p.active ? "active" : "not active", ref p.active, ref currChanged, 18);
                EditorGUIUtils.Toggle("purchased in debug mode", ref p.debugModePurchased, ref currChanged);
                ShowScreenshot(ref p.appleReviewScreenshotPath, ref currChanged);
            }
            if (p.appleReviewScreenshotPath.IsNullOrEmpty())
            {
                p.appleReviewScreenshotPath = AbstractProductSettings.StubScreenshotPath;
                currChanged = true;
            }
            changed = currChanged;
            if (sb.Length > 0)
                EditorGUIUtils.Error(sb.ToString());
        }
        protected void ShowRemovesInterstitials(ref bool removesInterstitials, ref bool changed)
            => EditorGUIUtils.Toggle("removes interstitials", ref removesInterstitials, ref changed);
        public virtual bool ProductValid(TSettings product, StringBuilder sb = null)
        {
            var valid1 = DescriptionValid(product.title, product.description, sb);
            var valid2 = PriceValid(product, sb);
            var valid3 = ScreenshotValid(product, sb);
            return valid1 && valid2 && valid3;
        }
        bool PriceValid(AbstractProductSettings product, StringBuilder sb = null)
        {
            if (StorePriceTierUtils.ToApplePriceTier((decimal)product.price, product.GetProductTypeEditMode()) ==-1)
            {
                sb?.AppendLine($"price = {product.price} is invalid for apple or google");
                return false;
            }
            return true;
        }
        protected virtual void ShowCertainProduct(TSettings p, ref bool changed) { }
        protected void ShowTitleDescription(ref string title, ref string description, ref bool changed, StringBuilder sb = null)
        {
            EditorGUIUtils.TextField("title", ref title, ref changed);
            EditorGUIUtils.TextField("description", ref description, ref changed);
            DescriptionValid(title, description, sb);
        }
        public static bool DescriptionValid(string title, string description, StringBuilder sb = null)
        {
            var valid = true;
            if (string.IsNullOrEmpty(title))
            {
                sb?.AppendLine("title not set");
                valid = false;
            } else if (title.Length < 2)
            {
                sb?.AppendLine("Title must be at least 2 characters long.");
                valid = false;
            } else if (title.Length > 25)
            {
                sb?.AppendLine("Title must be not longer than 25 chars."); // https://support.google.com/googleplay/android-developer/answer/140504
                valid = false;
            }

            if (string.IsNullOrEmpty(description))
            {
                sb?.AppendLine("description not set");
                valid = false;
            }
            else if (description.Length < 10)
            {
                sb?.AppendLine("Description must be at least 10 characters long.");
                valid = false;
            } else if (description.Length > 45)
            {
                sb?.AppendLine("Description must be no longer than 45 chars.");
                valid = false;
            }
            return valid;
        }
        void ShowScreenshot(ref string appleReviewScreenshotPath, ref bool changed)
        {
            var screenshot = GetScreenshot(appleReviewScreenshotPath);
            var newScreenshot = EditorGUILayout.ObjectField("apple review screenshot", screenshot, typeof(Texture2D), false);
            if (newScreenshot != screenshot)
            {
                changed = true;
                appleReviewScreenshotPath = AssetDatabase.GetAssetPath(newScreenshot);
            }
        }

        Dictionary<string, Texture2D> screenshotsCache;
        Texture2D GetScreenshot(string path)
        {
            if (screenshotsCache == null)
                screenshotsCache = new Dictionary<string, Texture2D>();
            if (screenshotsCache.TryGetValue(path, out var screenshot))
                return screenshot;
            screenshot = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            screenshotsCache[path] = screenshot;
            return screenshot;
        }
        bool ScreenshotValid(AbstractProductSettings product, StringBuilder sb = null)
        {
            if (!ios) return true;
            if (product.appleReviewScreenshotPath == AbstractProductSettings.StubScreenshotPath)
            {
                sb?.AppendLine("apple review screenshot is not set");
                return false;
            }
            if (string.IsNullOrEmpty(product.appleReviewScreenshotPath))
            {
                sb?.AppendLine("apple review screenshot is not set");
                return false;
            }
            var screenshot = GetScreenshot(product.appleReviewScreenshotPath);
            if (screenshot == null)
            {
                sb?.AppendLine($"apple screenshot does not exist on path {product.appleReviewScreenshotPath}");
                return false;
            }
            if (product.appleReviewScreenshotPath.Contains(' '))
            {
                sb?.AppendLine("apple review screenshot cant contain spaces");
                return false;
            }
            // https://help.apple.com/app-store-connect/#/dev84b80958f
            // A screenshot of the in-app purchase that represents the item being sold.For example, if it's a book, you can submit a screenshot of the book image. Or you can submit a screenshot of the purchase page. This screenshot is used for Apple’s review only and is not displayed on the App Store.
            // Screenshots requirements are outlined below:
            // After you upload an app review image, you can replace it, but you can’t remove it.You can’t update the screenshot while your in-app purchase is in review.
            var valid = allowedScreenshotsSizes.Any(allowed => allowed.x == screenshot.width && allowed.y == screenshot.height);
            if (!valid)
            {
                sb?.AppendLine($"iap screenshot for {product.productId} is {screenshot.width}X{screenshot.height}, but allowed options are:\n" +
                    $"{allowedScreenshotsSizes.PrintCollection("\n", toString:s=>$"{s.x}x{s.y}")}");
                return false;
            }
            return true;
        }
        static List<Vector2Int> allowedScreenshotsSizes = new List<Vector2Int> {
            new Vector2Int(1334, 750),
            new Vector2Int(750,1334),
            new Vector2Int(2732,2048),
            new Vector2Int(2048,2732),
            new Vector2Int(2208,1242),
            new Vector2Int(1242,2208),
            new Vector2Int(2688,1242),
            new Vector2Int(1242,2688),
            new Vector2Int(2778,1284),
            new Vector2Int(1284,2778),
            new Vector2Int(2388,1668),
            new Vector2Int(1668,2388),
            new Vector2Int(1640,2360),
            new Vector2Int(2360,1640),
            new Vector2Int(960,640),
            new Vector2Int(960,600),
            new Vector2Int(640,960),
            new Vector2Int(640,920),
            new Vector2Int(1136,640),
            new Vector2Int(1136,600),
            new Vector2Int(640,1136),
            new Vector2Int(640,1096),
            new Vector2Int(2436,1125),
            new Vector2Int(1125,2436),
            new Vector2Int(2340,1080),
            new Vector2Int(1080,2340),
            new Vector2Int(312,390),
            new Vector2Int(1024,768),
            new Vector2Int(1024,748),
            new Vector2Int(768,1024),
            new Vector2Int(768,1004),
            new Vector2Int(2048,1536),
            new Vector2Int(2048,1496),
            new Vector2Int(1536,2048),
            new Vector2Int(1536,2008),
            new Vector2Int(368,448),
            new Vector2Int(2224,1668),
            new Vector2Int(1668,2224)
        };
        void ValidateScreenshot(AbstractProductSettings product, ref bool changed)
        {
            if (string.IsNullOrEmpty(product.appleReviewScreenshotPath))
                return;
            var relativePath = StringUtils.EnsurePathRelative(product.appleReviewScreenshotPath);
            if (relativePath == product.appleReviewScreenshotPath)
                return;
            product.appleReviewScreenshotPath = relativePath;
            changed = true;
        }
    }

    public abstract class NonSubscriptionProductSettingsEditor<TSettings> : AbstractProductSettingsEditor<TSettings>
        where TSettings : AbstractProductSettings, new()
    {
        public override bool ProductValid(TSettings product, StringBuilder sb = null)
        {
            var valid1 = base.ProductValid(product, sb);
            var valid2 = ProductIdSuffixValid(product.productIdSuffix, sb);
            return valid1;
        }
        bool ProductIdSuffixValid(string productIdSuffix, StringBuilder sb = null)
        {
            if (string.IsNullOrEmpty(productIdSuffix))
            {
                sb?.AppendLine("product id suffix empty");
                return false;
            }

            // https://docs.unity3d.com/Manual/UnityIAPDefiningProducts.html
            productIdSuffix = productIdSuffix.Replace("_", "");
            //productIdSuffix = productIdSuffix.Replace(".", "");
            var isValid = productIdSuffix.DigitsCount() + productIdSuffix.LowerCaseLettersCount() == productIdSuffix.Length;
            if (!isValid)
                sb?.AppendLine("Product id may only contain lowercase letters, numbers, underscores, or periods");
            return isValid;
        }
    }
    public class ConsumableProductSettingsEditor : NonSubscriptionProductSettingsEditor<ConsumableProductSettings>
    {
        public static ConsumableProductSettingsEditor instance { get; private set; } = new ConsumableProductSettingsEditor();
        protected override List<ConsumableProductSettings> products => config.consumables;
        protected override string listName => "consumable";
    }
    public class NonConsumableProductSettingsEditor : NonSubscriptionProductSettingsEditor<NonConsumableProductSettings>
    {
        public static NonConsumableProductSettingsEditor instance { get; private set; } = new NonConsumableProductSettingsEditor();
        protected override List<NonConsumableProductSettings> products => config.nonConsumables;
        protected override string listName => "non-consumable";

        protected override void ShowCertainProduct(NonConsumableProductSettings p, ref bool changed)
            => ShowRemovesInterstitials(ref p.removesInterstitials, ref changed);
    }
    public class SubscriptionProductSettingsEditor : AbstractProductSettingsEditor<SubscriptionProductSettings>
    {
        public static SubscriptionProductSettingsEditor instance { get; private set; } = new SubscriptionProductSettingsEditor();
        protected override List<SubscriptionProductSettings> products => config.subscription.products;
        protected override string listName => "subscription product";
        protected override void ShowCertainProduct(SubscriptionProductSettings p, ref bool changed) {
            EditorGUIUtils.Popup("duration", ref p.duration, ref changed);
            ShowRemovesInterstitials(ref p.removesInterstitials, ref changed);
        }
    }

    public static class AbstractProductSettingsEditorUtils
    {
        public static ProductType GetProductTypeEditMode(this AbstractProductSettings product)
            => AbstractProductSettingsUtils.GetProductType(product, SettingsInEditor<IAPSettings>.instance);
    }
}
#endif