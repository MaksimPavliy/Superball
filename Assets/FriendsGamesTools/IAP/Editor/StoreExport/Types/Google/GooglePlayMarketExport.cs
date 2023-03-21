#if IAP
using FriendsGamesTools.EditorTools;
using FriendsGamesTools.Integrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace FriendsGamesTools.IAP
{
    public class GooglePlayMarketExport : AppStoreExport
    {
        new GooglePlayMarketCredentials credentials => base.credentials.googlePlayMarket;
        string packageId => PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
        public static GooglePlayMarketExport instance { get; private set; } = new GooglePlayMarketExport();
        public override string storeName => "Google Play Market";
        protected override void OnCustomGUI(ref bool changed)
        {
            base.OnCustomGUI(ref changed);
            if (!string.IsNullOrEmpty(credentials.refresh_token))
                EditorGUIUtils.InHorizontal(() => {
                    EditorGUIUtils.ShowValid(true);
                    GUILayout.Label("Logged in");
                });
            AuthCodeGUI(ref changed);
        }
        public override bool ExportValid(StringBuilder sb = null) => true;
        protected override async Task<bool> Exporting()
        {
            // Auth.
            var (success, productsInStore) = await EnsureAuthed();
            if (!success)
                return false;

            success = await UpdateProducts(productsInStore);
            if (!success)
                return false;
            //await CreateOrUpdateConsumableNonConsumableProduct(config.consumables.Last(), true);
            //await DeleteProduct(config.consumables.Last().productId);

            // Success.
            return true;
        }

        #region Update products on store
        async Task<(bool, List<GoogleIAPProduct>)> ListProducts()
        {
            Debug.Log($"listing products");
            var (success, text) = await APIRequest(APIMethods.list, "", "", true);
            if (!success)
                return (false, null);
            var response = ListIAPsResponse.Parse(text);
            return (true, response);
        }
        async Task<bool> UpdateProducts(List<GoogleIAPProduct> productsInStore)
        {
            bool success = true;
            var productsToAdd = config.allProducts.Clone();
            foreach (var productInStore in productsInStore)
            {
                var productInConfig = config.allProducts.Find(p => p.productId == productInStore.sku);
                if (productInConfig == null) // Remove products.
                    success &= await DeleteProduct(productInStore);
                else // Update products.
                    success &= await AddOrUpdateProduct(productInConfig, false);
                productsToAdd.Remove(productInConfig);
            }
            // Add products.
            foreach (var productInConfig in productsToAdd)
                success &= await AddOrUpdateProduct(productInConfig, true);
            return success;
        }
        async Task<bool> DeleteProduct(GoogleIAPProduct productInStore)
        {
            Debug.Log($"deleting {productInStore.sku}");
            bool success;
            if (productInStore is SubscriptionGoogleIAPProduct subscription)
            {
                // Subscription cant be deleted ever. Google is investigating it. 
                // We just consider it deleted and create new version.
                const string deleted = "DELETED";
                subscription.listings.en_US.title = deleted;
                subscription.listings.en_US.description = deleted;
                subscription.trialPeriod = DurationISO8601.Zero;
                success = await AddOrUpdateProduct(productInStore, false);
            }
            else
                (success, _) = await APIRequest(APIMethods.delete, productInStore.sku, "", false);
            if (success)
                OnProductIdDeleted(productInStore.sku);
            return success;
        }
        void OnProductIdDeleted(string productId)
        {
            config.OnProductIdDeleted(productId);
            EditorUtils.SetDirty(config);
        }
        async Task<bool> AddOrUpdateProduct(AbstractProductSettings product, bool createOrUpdate)
        {
            Debug.Log($"{(createOrUpdate?"adding":"updating")} {product.productId}");
            //var managedProductInConfig = product as ProductSettings;
            var subscriptionProductInConfig = product as SubscriptionProductSettings;

            SubscriptionGoogleIAPProduct subscriptionProductInStore;
            GoogleIAPProduct productInStore;
            if (subscriptionProductInConfig != null)
            {
                subscriptionProductInStore = new SubscriptionGoogleIAPProduct();
                productInStore = subscriptionProductInStore;
                productInStore.purchaseType = GoogleIAPProduct.PurchaseType.subscription.ToString();
                // Subscription-specific.
                subscriptionProductInStore.subscriptionPeriod = subscriptionProductInConfig.duration.ToGooglePlayMarket().ToISO8601();
                subscriptionProductInStore.trialPeriod = config.subscription.freeTrialExists ? 
                    config.subscription.GetFreeTrialDuration().ToGooglePlayMarket().ToISO8601() : DurationISO8601.Zero;
                subscriptionProductInStore.gracePeriod = config.subscription.gracePeriod.ToGooglePlayMarket().ToISO8601();

                productInStore.listings = new GoogleIAPTitleDesc { en_US = new GoogleIAPTitleDescLocalized {
                        title = $"{config.subscription.title} {product.title}",
                        description = $"{config.subscription.description} {product.description}"
                } };
            } else
            {
                subscriptionProductInStore = null;
                productInStore = new GoogleIAPProduct();
                productInStore.purchaseType = GoogleIAPProduct.PurchaseType.managedUser.ToString();

                productInStore.listings = new GoogleIAPTitleDesc { en_US = new GoogleIAPTitleDescLocalized {
                        title = product.title,
                        description = product.description
                } };
            }

            productInStore.defaultLanguage = "en-US";
            productInStore.defaultPrice = new GoogleIAPPriceLocalized((decimal)product.price, CurrencyISO4217.UAH);
            
            // Dont fill prices, they-ll be updated automatically.
            //productInStore.prices = new GoogleIAPPrices { US = new GoogleIAPPriceLocalized(product.price, CurrencyISO4217.USD) };
            productInStore.packageName = packageId;
            productInStore.sku = product.productId;
            productInStore.status = (product.active ? GoogleIAPProduct.Status.active : GoogleIAPProduct.Status.inactive).ToString();
            
            return await AddOrUpdateProduct(productInStore, createOrUpdate);
        }
        async Task<bool> AddOrUpdateProduct(GoogleIAPProduct productInStore, bool createOrUpdate)
        {
            var json = productInStore.ToJSON();
            json = json.Replace(",\"prices\":{\"US\":{\"priceMicros\":0,\"currency\":\"\"}}", ",\"prices\":{}");
            json = json.Replace($",\"trialPeriod\":\"{DurationISO8601.Zero}\"", "");
            //json = "{\"packageName\":\"com.friendsgamesincubator.fgttest\",\"sku\":\"com.friendsgamesincubator.fgttest.consumable3\",\"status\":\"active\",\"purchaseType\":\"managedUser\",\"defaultPrice\":{\"priceMicros\":\"25000000\",\"currency\":\"UAH\"},\"prices\":{\"US\":{\"priceMicros\":\"990000\",\"currency\":\"USD\"}},\"listings\":{\"en-US\":{\"title\":\"consumable 3 title\",\"description\":\"consumable 3 description\"}},\"defaultLanguage\":\"en-US\"}";
            var (success, _) = await APIRequest(createOrUpdate ? APIMethods.insert : APIMethods.update, productInStore.sku, json, false);
            return success;
        }
        #endregion

        #region API methods
        enum APIMethods { delete, get, insert, list, patch, update }
        async Task<(bool, string)> APIRequest(APIMethods method, string productSKU, string json, bool errorIsOk)
        {
            Debug.Log($"Google API request {method} {productSKU}\njson = \n{json}");
            var url = $"https://www.googleapis.com/androidpublisher/v3/applications";
            UnityWebRequest CreateRequest()
            {
                switch (method)
                {
                    case APIMethods.delete: return UnityWebRequest.Delete($"{url}/{packageId}/inappproducts/{productSKU}");
                    case APIMethods.get: return UnityWebRequest.Get($"{url}/{packageId}/inappproducts/{productSKU}");
                    case APIMethods.insert: return UnityWebRequest.Post($"{url}/{packageId}/inappproducts?autoConvertMissingPrices=true", json);
                    case APIMethods.list: return UnityWebRequest.Get($"{url}/{packageId}/inappproducts");
                    default:
                    case APIMethods.patch: throw new NotImplementedException();
                    case APIMethods.update:
                        return UnityWebRequest.Put($"{url}/{packageId}/inappproducts/{productSKU}?autoConvertMissingPrices=true", json);
                }
            }

            using (UnityWebRequest www = CreateRequest())
            {
                if (method == APIMethods.insert || method == APIMethods.update)
                {
                    var bytes = Encoding.UTF8.GetBytes(json);
                    www.uploadHandler = new UploadHandlerRaw(bytes);
                    www.uploadHandler.contentType = "application/json";
                }
                www.SetRequestHeader("authorization", $"Bearer {credentials.access_token}");
                await www.SendWebRequestInEditor();

                if (www.isNetworkError || www.isHttpError)
                {
                    var error = $"{www.error}\n{www.downloadHandler?.text}";
                    if (error.Contains("Cannot reuse deleted SKU"))
                        OnProductIdDeleted(productSKU);
                    if (errorIsOk)
                        Debug.Log(error);
                    else
                        Debug.LogError(error);
                    return (false, error);
                }
                else
                {
                    if (www.downloadHandler != null)
                    {
                        Debug.Log(www.downloadHandler.text);
                        return (true, www.downloadHandler.text);
                    }
                    else
                    {
                        Debug.Log("www request success");
                        return (true, string.Empty);
                    }
                }
            }
        }
        #endregion

        #region Authing
        async Task<(bool, List<GoogleIAPProduct>)> EnsureAuthed()
        {
            bool success = false;
            List<GoogleIAPProduct> currProducts = null;

            // Try current access token.
            if (!string.IsNullOrEmpty(credentials.access_token))
            {
                (success, currProducts) = await ListProducts();
                if (success)
                    return (true, currProducts); // Current access token is ok.
            }

            // Curr access token is not valid.
            credentials.access_token = string.Empty;

            // Try refresh token.
            if (!string.IsNullOrEmpty(credentials.refresh_token))
            {
                success = await RefreshToken();
                if (!success)
                    return (false, null);

                // Try refreshed token.
                (success, currProducts) = await ListProducts();
                if (success)
                    return (success, currProducts); // Refreshed access token is ok.
            }

            // Token cant be refreshed.
            credentials.refresh_token = string.Empty;

            // Start authing over.

            // Step 1: Generate a code verifier and challenge.
            //credentials.code_verifier = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

            // Step 2: Send a request to Google's OAuth 2.0 server.
            if (string.IsNullOrEmpty(credentials.code))
                await GetAuthCode();

            // Step 3. Get access token.
            success = await GetAccessToken();
            if (!success)
                return (false, null);

            // Success.
            return await ListProducts();
        }
        async Task GetAuthCode()
        {
            var googleOAuthRequestURL = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                $"client_id={credentials.client_id}&" +
                $"redirect_uri={credentials.redirect_uri_escaped}&" +
                $"response_type=code&" +
                $"scope={credentials.scope_escaped}";
            Debug.Log($"googleOAuthRequestURL = {googleOAuthRequestURL}");
            isGettingCode = true;
            Application.OpenURL(googleOAuthRequestURL);
            while (string.IsNullOrEmpty(credentials.code))
                await Awaiters.EndOfFrame;
            isGettingCode = false;
        }
        void AuthCodeGUI(ref bool changed)
        {
            if (!isGettingCode)
                return;
            EditorGUIUtils.TextField("url from browser after access approved", ref callbackUrl, ref changed);
            var parsedAuthCode = GetCodeFromCallbackUrl(callbackUrl);
            if (string.IsNullOrEmpty(parsedAuthCode))
                return;
            credentials.code = parsedAuthCode;
        }
        string GetCodeFromCallbackUrl(string url)
        {
            if (url == null)
                return string.Empty;
            const string codeEquals = "code=";
            var startInd = url.IndexOf(codeEquals);
            if (startInd == -1)
                return string.Empty;
            startInd += codeEquals.Length;
            var endInd = url.IndexOf("&");
            if (endInd == -1)
                endInd = url.Length;
            var count = endInd - startInd;
            var code = url.Substring(startInd, count);
            Debug.Log($"code = {code}");
            return code;
        }
        bool isGettingCode;
        string callbackUrl = "";
        async Task<bool> GetAccessToken()
        {
            WWWForm form = new WWWForm();
            form.AddField("grant_type", "authorization_code");
            form.AddField("code", credentials.code);
            form.AddField("client_id", credentials.client_id);
            form.AddField("client_secret", credentials.client_secret);
            form.AddField("redirect_uri", credentials.redirect_uri);
            //form.AddField("code_verifier", credentials.code_verifier); // "error": "invalid_grant", "error_description": "code_verifier or verifier is not needed."
            form.AddField("scope", credentials.scope);
            Debug.Log($"code={credentials.code}\n" +
                $"client_id={credentials.client_id}\n" +
                $"client_secret = {credentials.client_secret}\n" +
                $"redirect_uri = {credentials.redirect_uri}");
            //form.AddField("autoConvertMissingPrices", "true");
            // https://www.googleapis.com/auth/androidpublisher

            var url = $"https://oauth2.googleapis.com/token";// "https://accounts.google.com/o/oauth2/token";// ;// $"https://www.googleapis.com/androidpublisher/v3/applications/{Application.identifier}/inappproducts";
            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                www.uploadHandler.contentType = "application/x-www-form-urlencoded";// ; charset=utf-8";//"application/json";  
                await www.SendWebRequestInEditor();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogError($"cant get access token:\n{www.error}\n{www.downloadHandler.text}");
                    return false;
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    var answer = TokenAnswer.Parse(www.downloadHandler.text);
                    if (string.IsNullOrEmpty(answer.access_token) || string.IsNullOrEmpty(answer.refresh_token))
                    {
                        Debug.LogError($"invalid access token answer:\n{www.downloadHandler.text}");
                        return false;
                    }
                    credentials.access_token = answer.access_token;
                    credentials.refresh_token = answer.refresh_token;
                    return true;
                }
            }
        }
        async Task<bool> RefreshToken()
        {
            const string url = "https://accounts.google.com/o/oauth2/token";
            WWWForm form = new WWWForm();
            form.AddField("grant_type", "refresh_token");
            form.AddField("client_id", credentials.client_id);
            form.AddField("client_secret", credentials.client_secret);
            form.AddField("refresh_token", credentials.refresh_token);
            Debug.Log($"Refreshing token\n" +
                $"client_id={credentials.client_id}\n" +
                $"client_secret = {credentials.client_secret}\n" +
                $"refresh_token = {credentials.refresh_token}");
            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                www.uploadHandler.contentType = "application/x-www-form-urlencoded";// ; charset=utf-8";//"application/json";  
                await www.SendWebRequestInEditor();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogError($"cant refresh access token:\n{www.error}\n{www.downloadHandler.text}");
                    return false;
                }
                else
                {
                    Debug.Log(www.downloadHandler.text);
                    var answer = TokenAnswer.Parse(www.downloadHandler.text);
                    if (string.IsNullOrEmpty(answer.access_token))
                    {
                        Debug.LogError($"invalid refresh access token answer:\n{www.downloadHandler.text}");
                        return false;
                    }
                    credentials.access_token = answer.access_token;
                    return true;
                }
            }
        }
        #endregion
    }

    [Serializable] public class GooglePlayMarketCredentials
    {
        public string code = "";
        public readonly string client_id = "545859707861-92rq64i2rs47t72akehbgt7lst0rripi.apps.googleusercontent.com";
        public readonly string redirect_uri = "http://localhost";
        public readonly string scope = "https://www.googleapis.com/auth/androidpublisher";
        public readonly string client_secret = "peJ_TxXd5glcdcA4qzvbtQAY";

        // For remembering not to use UnityWebRequest.EscapeURL. it makes %f but %F is only accepted here.
        static string URLEscape(string text) => Uri.EscapeDataString(text);
        public string redirect_uri_escaped => URLEscape(redirect_uri);
        public string scope_escaped => URLEscape(scope);

        public string access_token = "";
        public string refresh_token = "";
    }

    [Serializable] public class TokenAnswer
    {
        public string access_token;
        public string token_type;
        public int expires_in;
        public string refresh_token;

        public static TokenAnswer Parse(string json) => JsonUtility.FromJson<TokenAnswer>(json);
    }

    [Serializable] public class ListIAPsResponse
    {
        public List<SubscriptionGoogleIAPProduct> inappproduct;

        public static List<GoogleIAPProduct> Parse(string json)
        {
            json = GoogleIAPTitleDesc.UnscreenJson(json);
            var response = JsonUtility.FromJson<ListIAPsResponse>(json);
            var list = new List<GoogleIAPProduct>();
            foreach (var currItem in response.inappproduct)
            {
                GoogleIAPProduct item;
                if (!currItem.isSubscription)
                    item = currItem.ToNonSubscription();
                else
                    item = currItem;
                list.Add(item);
            }
            return list;
        }
        public string ToJSON()
        {
            var json = JsonUtility.ToJson(this);
            json = GoogleIAPTitleDesc.ScreenJson(json);
            return json;
        }
    }
    [Serializable] public class GoogleIAPProduct
    {
        public string packageName; // com.friendsgamesincubator.fgttest
        public string sku; // com.friendsgamesincubator.fgttest.consumable1
        public string status; // active inactive
        public enum Status { active, inactive }
        public string purchaseType; // managedUser subscription
        public GoogleIAPPriceLocalized defaultPrice;
        public GoogleIAPPrices prices;
        public GoogleIAPTitleDesc listings;
        public string defaultLanguage; //  BCP 47 language code. https://tools.ietf.org/html/bcp47

        public enum PurchaseType { managedUser, subscription }
        public bool isSubscription => purchaseType == PurchaseType.subscription.ToString();

        public static GoogleIAPProduct Parse(string json)
        {
            json = GoogleIAPTitleDesc.UnscreenJson(json);
            return JsonUtility.FromJson<GoogleIAPProduct>(json);
        }
        public virtual string ToJSON()
        {
            var json = JsonUtility.ToJson(this);
            json = GoogleIAPTitleDesc.ScreenJson(json);
            return json;
        }
    }
    [Serializable]
    public class SubscriptionGoogleIAPProduct : GoogleIAPProduct
    {
        public SubscriptionGoogleIAPProduct() { purchaseType = "subscription"; }
        public string subscriptionPeriod; // P1W P1M P1Y 30d = P4W2D   ISO 8601
        public string trialPeriod; // P1W     ISO 8601 https://en.wikipedia.org/wiki/ISO_8601
        public string gracePeriod; // P3D     ISO 8601

        public static new SubscriptionGoogleIAPProduct Parse(string json)
        {
            json = GoogleIAPTitleDesc.UnscreenJson(json);
            return JsonUtility.FromJson<SubscriptionGoogleIAPProduct>(json);
        }
        public override string ToJSON()
        {
            var json = JsonUtility.ToJson(this);
            json = GoogleIAPTitleDesc.ScreenJson(json);
            return json;
        }
        public GoogleIAPProduct ToNonSubscription() => GoogleIAPProduct.Parse(ToJSON());
    }
    [Serializable] public class GoogleIAPPrices
    {
        public GoogleIAPPriceLocalized US;
    }
    [Serializable] public class GoogleIAPPriceLocalized
    {
        public long priceMicros; // 25 000 000 = 25 uah.
        public string currency; // UAH USD.    ISO 4217. https://www.iso.org/iso-4217-currency-codes.html
        public GoogleIAPPriceLocalized(decimal priceUSD, CurrencyISO4217 currency)
        {
            var rate = GetRate(currency);
            this.currency = currency.ToString();
            priceMicros = (long)Math.Round(1000000d * (double)(priceUSD * rate));
            // Price rounds to cents.
            priceMicros -= priceMicros % 10000;
        }
        private decimal GetRate(CurrencyISO4217 currency)
        {
            switch (currency)
            {
                default: throw new Exception();
                case CurrencyISO4217.USD: return 1;
                case CurrencyISO4217.UAH: return 25;
            }
        }
    }
    public enum CurrencyISO4217
    {
        UAH, USD
    }
    [Serializable] public class GoogleIAPTitleDesc
    {
        public GoogleIAPTitleDescLocalized en_US;
        public static string ScreenJson(string json) => json.Replace("en_US", "en-US");
        public static string UnscreenJson(string json) => json.Replace("en-US", "en_US");
    }
    [Serializable] public class GoogleIAPTitleDescLocalized
    {
        public string title;
        public string description;
    }

    public static class DurationISO8601
    {
        public static string ToISO8601(this GoogleSubscriptionDuration duration)
        {
            switch (duration)
            {
                default:
                case GoogleSubscriptionDuration.Weekly: return "P1W";
                case GoogleSubscriptionDuration.OneMonth: return "P1M";
                case GoogleSubscriptionDuration.ThreeMonths: return "P3M";
                case GoogleSubscriptionDuration.SixMonths: return "P6M";
                case GoogleSubscriptionDuration.Annual: return "P1Y";
            }
        }
        public const string Zero = "P0D";
    }
}
#endif