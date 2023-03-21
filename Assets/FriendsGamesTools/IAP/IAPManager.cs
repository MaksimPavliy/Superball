#if IAP
using FriendsGamesTools.EditorTools.BuildModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace FriendsGamesTools.IAP
{
    public abstract partial class IAPManager : MonoBehaviour, IStoreListener
    {
        #region IStoreListener
        private IStoreController controller;
        private IExtensionProvider extensions;
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            inited = true;
            isIniting = false;
            this.controller = controller;
            this.extensions = extensions;
            InitRuntimeProducts();
            InitDebugMode();
            TriggerChanges();
            if (logging)
                Debug.Log($"OnInitialized called");
        }
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            initFailed = true;
            isIniting = false;
            Debug.Log($"OnInitializeFailed called with {error}");
        }
        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            var productId = i.definition.id;
            var log = $"purchase failed, reason = {p}";
            Debug.Log(log);
#if INFO_NOTIFICATION
            UI.InfoNotificationView.Show(log);
#endif
            ProcessResponse(productId, false);
        }
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            if (logging)
                Debug.Log($"ProcessPurchase = {e.purchasedProduct.definition.id} {e.purchasedProduct.receipt}");
            var isValidPurchase = IsValidPurchase(e);
            var productId = e.purchasedProduct.definition.id;
            if (isValidPurchase)
                ProcessValidPurchase(productId);
            else
                ProcessResponse(productId, false);
            return PurchaseProcessingResult.Complete;
        }
        #endregion

        #region Runtime products data
        public bool interstitialsRemoved => products.Any(p => p.config.removesInterstitials && p.active);
        public AbstractProductSettings removeInterstitialsConfig => config.allProducts.Find(p => p.removesInterstitials);
        private IAppleExtensions m_AppleExtensions;
        private Dictionary<string, IAPProductData> realDataDict = new Dictionary<string, IAPProductData>();
        public IEnumerable<IAPProductData> products => mode == Mode.Real ? realDataDict.Values : mockedProducts;
        public IAPProductData GetProductData(string productId)
        {
            if (mode == Mode.Mocked)
                return GetMockedData(productId);
            realDataDict.TryGetValue(productId, out var data);
            return data;
        }

        public IAPSubscriptionData GetSubscriptionData(string productId)
        {
            if (mode == Mode.Mocked)
                return GetMockedSubscriptionData(productId);
            return GetProductData(productId) as IAPSubscriptionData;
        }

        public IAPSubscriptionData GetActiveSubscriptionData()
        {
            if (mode == Mode.Mocked)
                return GetMockedActiveSubscriptionData();
            return realDataDict.Values.Find(p => p is IAPSubscriptionData s && s.active) as IAPSubscriptionData;
        }

        void InitRuntimeProducts()
        {
            realDataDict.Clear();
            m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
            var introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();

            var sb = new StringBuilder();
            sb.AppendLine("Available items:");
            foreach (var item in controller.products.all)
            {
                if (item.availableToPurchase)
                {
                    sb.AppendLine($" - {item.definition.id} {item.definition.type} enabled={item.definition.enabled} " +
                        $"hasReceipt={item.hasReceipt} {(item.hasReceipt ? item.receipt : "")} transactionID={item.transactionID} " +
                        $"{item.metadata.isoCurrencyCode} {item.metadata.localizedTitle} {item.metadata.localizedDescription} " +
                        $"{item.metadata.localizedPriceString}");
                    //metadataDict.Add(item.definition.id, item.metadata);
                    // Set all these products to be visible in the user's App Store according to Apple's Promotional IAP feature
                    // https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/StoreKitGuide/PromotingIn-AppPurchases/PromotingIn-AppPurchases.html
                    m_AppleExtensions.SetStorePromotionVisibility(item, AppleStorePromotionVisibility.Show);

                    switch (item.definition.type)
                    {
                        case UnityEngine.Purchasing.ProductType.Subscription:
                            var intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
                            var remainingTime = -1f;
                            if (item.hasReceipt)
                            {
                                var subscriptionManager = new SubscriptionManager(item, intro_json);
                                //subscriptionsDict.Add(item.definition.id, subscriptionManager);
                                var subscriptionInfo = subscriptionManager.getSubscriptionInfo();
                                sb.AppendLine("SubscriptionInfo:");
                                sb.AppendLine("product id is: " + subscriptionInfo.getProductId());
                                sb.AppendLine("purchase date is: " + subscriptionInfo.getPurchaseDate());
                                sb.AppendLine("subscription next billing date is: " + subscriptionInfo.getExpireDate());
                                sb.AppendLine("is subscribed? " + subscriptionInfo.isSubscribed().ToString());
                                sb.AppendLine("is expired? " + subscriptionInfo.isExpired().ToString());
                                sb.AppendLine("is cancelled? " + subscriptionInfo.isCancelled());
                                sb.AppendLine("product is in free trial peroid? " + subscriptionInfo.isFreeTrial());
                                sb.AppendLine("product is auto renewing? " + subscriptionInfo.isAutoRenewing());
                                sb.AppendLine("subscription remaining valid time until next billing date is: " + subscriptionInfo.getRemainingTime());
                                sb.AppendLine("is this product in introductory price period? " + subscriptionInfo.isIntroductoryPricePeriod());
                                sb.AppendLine("the product introductory localized price is: " + subscriptionInfo.getIntroductoryPrice());
                                sb.AppendLine("the product introductory price period is: " + subscriptionInfo.getIntroductoryPricePeriod());
                                sb.AppendLine("the number of product introductory price period cycles is: " + subscriptionInfo.getIntroductoryPricePeriodCycles());
                                if (subscriptionInfo.isSubscribed() == Result.True)
                                    remainingTime = (float)subscriptionInfo.getRemainingTime().TotalSeconds;
                            }
                            else
                                sb.AppendLine("has no receipt");
                            realDataDict.Add(item.definition.id,
                                new IAPSubscriptionData(item.definition.id, item.metadata.localizedTitle, item.metadata.localizedDescription,
                                item.metadata.localizedPrice, item.metadata.isoCurrencyCode, remainingTime));
                            break;
                        case UnityEngine.Purchasing.ProductType.Consumable:
                            realDataDict.Add(item.definition.id,
                                new IAPConsumableData(item.definition.id, item.metadata.localizedTitle, item.metadata.localizedDescription,
                                item.metadata.localizedPrice, item.metadata.isoCurrencyCode));
                            break;
                        case UnityEngine.Purchasing.ProductType.NonConsumable:
                            var owned = item.hasReceipt;
                            realDataDict.Add(item.definition.id,
                                new IAPNonConsumableData(item.definition.id, item.metadata.localizedTitle, item.metadata.localizedDescription,
                                item.metadata.localizedPrice, item.metadata.isoCurrencyCode, owned));
                            break;
                    }
                }
            }
            if (logging)
                Debug.Log(sb.ToString());
        }

        public bool subscriptionActive => GetActiveSubscriptionData() != null;
        public bool subscriptionAvailable => config.subscription.exists && !subscriptionActive;
        // So far there's no way to check this.
        //public bool freeTrialAvailable => subscriptionAvailable && config.subscription.freeTrialExists; // TODO: check free trial still available to curr user.
        public float subscriptionRemainingTime => GetActiveSubscriptionData()?.remainingTime ?? 0;
        #endregion

        #region Receipt validation
        public static Type GooglePlayTangleType { get; } = ReflectionUtils.GetTypeByName("GooglePlayTangle", true);
        public static Type AppleTangleType { get; } = ReflectionUtils.GetTypeByName("AppleTangle", true);
        bool IsValidPurchase(PurchaseEventArgs e)
        {
            if (!config.validatePurchases)
                return true;
            var isValidPurchase = true; // Presume valid for platforms with no R.V.

            // Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
            // Prepare the validator with the secrets we prepared in the Editor
            // obfuscation window.

            var validator = new CrossPlatformValidator(
                (byte[])GooglePlayTangleType.CallStaticMethod("Data"),
                (byte[])AppleTangleType.CallStaticMethod("Data"),
                Application.identifier);

            try
            {
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(e.purchasedProduct.receipt);
                // For informational purposes, we list the receipt(s)
                if (logging)
                    Debug.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    if (logging)
                        Debug.Log(productReceipt.productID);
                    if (logging)
                        Debug.Log(productReceipt.purchaseDate);
                    if (logging)
                        Debug.Log(productReceipt.transactionID);
                }
            }
            catch (IAPSecurityException err)
            {
                Debug.Log($"Invalid receipt, not unlocking content,\n" +
                    $"err = {err.Message}\n" +
                    $"{err.InnerException}\n" +
                    $"receipt = {e.purchasedProduct.receipt}");
                isValidPurchase = false;
            }
#endif

            return isValidPurchase;
        }
        public void TriggerChanges()
        {
            onChanged?.Invoke();
        }
        #endregion

        #region UI responses
        Dictionary<string, Queue<Action<string, bool>>> Responses = new Dictionary<string, Queue<Action<string, bool>>>();
        protected virtual void OnPurchaseResponse(string productId, bool success) { }
        protected virtual void ProcessResponse(string productId, bool success)
        {
            if (Responses.TryGetValue(productId, out var productIdResponses) && productIdResponses.Count > 0)
                productIdResponses.Dequeue().Invoke(productId, true);
        }
        void AddResponse(string productId, Action<bool> UIResponse)
        {
            if (!Responses.TryGetValue(productId, out var productIdResponses))
                Responses.Add(productId, productIdResponses = new Queue<Action<string, bool>>());
            productIdResponses.Enqueue((string prodId, bool success) =>
            {
                OnPurchaseResponse(prodId, success);
                UIResponse?.Invoke(success);
            });
        }
        #endregion

        #region Restore purchases
        protected virtual bool manualRestorePurchasesInEditor => defaultManualRestorePurchases;
        private bool defaultManualRestorePurchases => TargetPlatformUtils.current == TargetPlatform.IOS;
        public bool manualRestorePurchases => Application.isEditor ? manualRestorePurchasesInEditor : defaultManualRestorePurchases;
        public virtual void OnRestorePurchasesClicked(Action<bool> onResponse = null)
        {
            if (!manualRestorePurchases)
            {
                Debug.LogError("can not restore purchases manually");
                onResponse?.Invoke(false);
                return;
            }
            if (mode == Mode.Mocked && config.debugMode == DebugRestorePurchasesMode.TestRestorePurchasesButton)
                MockRestoreTransactions(onResponse);
            else
            {
                if (m_AppleExtensions == null)
                    Debug.LogError("OnRestorePurchasesClicked m_AppleExtensions == null");
                else
                    m_AppleExtensions.RestoreTransactions(result => onResponse?.Invoke(result));
            }
        }
        #endregion

        #region Initing
        bool logging => config.logs;
        public static IAPManager instance { get; private set; }
        protected virtual void Awake()
        {
            instance = this;
        }
        protected virtual void Start()
        {
            InitIAP();
        }
        void InitIAP()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            IAPConfigurationHelper.PopulateConfigurationBuilder(ref builder, ProductCatalog.LoadDefaultCatalog());
            UnityPurchasing.Initialize(this, builder);
        }
        public bool inited { get; private set; }
        public bool initFailed { get; private set; }
        public bool isIniting { get; private set; } = true;
        void Update() => UpdateSubscriptinTimeout();
        #endregion

        #region Purchase user settings
        // Can be disabled in ios settings.
        protected virtual bool canMakePaymentsInEditor => true;
        public bool canMakePayments
        {
            get
            {
                if (Application.isEditor)
                    return canMakePaymentsInEditor;
                if (TargetPlatformUtils.current == TargetPlatform.IOS)
                {
                    var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
                    return builder.Configure<IAppleConfiguration>().canMakePayments;
                }
                return true;
            }
        }
        public IAPSettings config => IAPSettings.instance;
        #endregion

        #region Apply purchases
        public const string ApplyConsumablePurchasedMethodName = "ApplyConsumablePurchase";
        public const string ApplyNonConsumablePurchasedMethodName = "ApplyOnNonConsumablePurchase";
        public const string ApplySubscriptionPurchasedMethodName = "ApplySubscriptionPurchase";
        public const string ConsumableProductEnumName = "ConsumableProduct";
        public const string NonConsumableProductEnumName = "NonConsumableProduct";
        public static event Action onChanged;
        private void ProcessValidPurchase(string productId)
        {
            InitRuntimeProducts(); // This is needed to update info about subscriptions.
            CallGeneratedApplyPurchaseMethod(productId, ApplyConsumablePurchasedMethodName);
            CallGeneratedApplyPurchaseMethod(productId, ApplyNonConsumablePurchasedMethodName);
            CallGeneratedApplyPurchaseMethod(productId, ApplySubscriptionPurchasedMethodName);
            ProcessResponse(productId, true);
            onChanged?.Invoke();
        }

        private void CallGeneratedApplyPurchaseMethod(string productId, string methodName)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var onPurchaseMethod = GetType().GetMethod(methodName, flags);
            if (onPurchaseMethod == null)
                return;
            var methodParam = onPurchaseMethod.GetParameters()[0];
            var productEnum = methodParam.ParameterType;
            if (ReflectionUtils.TryParseEnum(AbstractProductSettings.GetProductSuffix(productId), productEnum, out var productIdAsEnumValue))
                onPurchaseMethod.Invoke(this, new object[] { productIdAsEnumValue });
            else if (methodName == ApplySubscriptionPurchasedMethodName && config.subscription.products.Any(p => p.productId == productId))
                onPurchaseMethod.Invoke(this, new object[] {
                    subscriptionActive ?
                    SubscriptionPurchaseType.ExistingSubscriptionProlongation:
                    SubscriptionPurchaseType.NewSubscriptionActivation });
        }
        #endregion

        #region Purchase methods
        public virtual async void OnPurchasePressed(string productId, Action<bool> onUIResponse = null)
        {
            try
            {
                AddResponse(productId, onUIResponse);
                await AsyncUtils.SimulateInternetDelayIfDebug();
                if (mode == Mode.Mocked)
                    OnMockedPurchase(productId);
                else
                {

                    controller.InitiatePurchase(productId);
                }
            }
            catch
            {
                if (controller == null)
                {
                    Debug.LogError("OnPurchasePressed Error: No Controller Assigned");
                }
                if(productId==null) Debug.LogError("OnPurchasePressed Error: product ID is empty");

                onUIResponse?.Invoke(false);
            }
        }
        public virtual void OnPurchaseSubscriptionPressed(SubscriptionDuration duration, Action<bool> onUIResponse = null)
        {
            var product = config.subscription.products.Find(p => p.active && p.duration == duration);
            OnPurchasePressed(product.productId, onUIResponse);
        }
        public virtual void OnPurchaseSubscriptionPressed(Action<bool> onUIResponse = null)
        {
            var product = config.subscription.products.Find(p => p.active);
            OnPurchasePressed(product.productId, onUIResponse);
        }
        #endregion

        #region Mocking
        public Mode mode { get; private set; }
        public void SetMode(Mode mode)
        {
            Debug.Assert(inited);
            this.mode = mode;
        }
        void InitDebugMode()
        {
            if (BuildModeSettings.release)
                mode = Mode.Real;
            else
                mode = Application.isEditor ? config.defaultModeInEditor : config.defaultModeInBuild;
            if (mode == Mode.Mocked)
                mockedSubscriptionEndTime = config.debugSubscriptionRemainingOnAppLaunch;
            if (mode == Mode.Mocked && config.debugMode == DebugRestorePurchasesMode.TestTransactionsOnInit)
                SendMockedPurchasesReceived();
        }
        async void MockRestoreTransactions(Action<bool> onResponse)
        {
            await Awaiters.SecondsRealtime(2);
            var success = await RestorePurchasesNativeWindow.Show();
            if (success)
                SendMockedPurchasesReceived();
            onResponse?.Invoke(success);
        }
        void SendMockedPurchasesReceived()
        {
            Debug.Assert(mode == Mode.Mocked);
            config.debugProductIdsToReceive.ForEach(productId =>
            {
                var product = config.GetProduct(productId);
                if (product == null || !product.active)
                    return;
                if (product is SubscriptionProductSettings s && !config.subscription.exists)
                    return;
                ProcessValidPurchase(productId);
            });
        }
        float mockedSubscriptionEndTime;
        float mockedSubscriptionRemainingTime => mockedSubscriptionEndTime - Time.realtimeSinceStartup;
        bool mockedSubscriptionActive => mockedSubscriptionRemainingTime > 0;
        string mockedSubscriptionActiveProductId
            => mockedSubscriptionActive ? config.subscription.products.Find(p => p.active)?.productId : string.Empty;
        public const string MockedCurrencyCode = "USD";
        private IEnumerable<IAPProductData> mockedProducts => config.allProducts.ConvertAll(c => GetMockedData(c.productId));
        private IAPProductData GetMockedData(string productId)
        {
            var product = config.GetProduct(productId);
            if (product == null)
                return null;
            var price = (decimal)product.price;
            if (product is SubscriptionProductSettings)
            {
                var isSubscribed = mockedSubscriptionActiveProductId == productId;
                var remainingTime = isSubscribed ? mockedSubscriptionRemainingTime : -1;
                return new IAPSubscriptionData(productId, product.title, product.description, price, MockedCurrencyCode, remainingTime);
            }
            else if (product is NonConsumableProductSettings)
                return new IAPNonConsumableData(productId, product.title, product.description, price, MockedCurrencyCode, product.debugModePurchased);
            else
                return new IAPConsumableData(productId, product.title, product.description, price, MockedCurrencyCode);
        }
        private IAPSubscriptionData GetMockedActiveSubscriptionData()
            => GetMockedSubscriptionData(mockedSubscriptionActiveProductId);
        private IAPSubscriptionData GetMockedSubscriptionData(string productId)
            => GetMockedData(productId) as IAPSubscriptionData;
        private async void OnMockedPurchase(string productId)
        {
            var product = GetProductData(productId);
            var success = await SpendMoneyNativeWindow.Show(PurchasableProductView.ToShownPrice(product.price, product.currency));
            if (!success)
                ProcessResponse(productId, false);
            else
            {
                if (product is IAPSubscriptionData subscription)
                {
                    var addedDuration = subscription.config.duration.ToSeconds();
                    if (mockedSubscriptionActive)
                        mockedSubscriptionEndTime += addedDuration;
                    else
                        mockedSubscriptionEndTime = addedDuration + Time.realtimeSinceStartup;
                }
                if (product is IAPNonConsumableData nonConsumable)
                {
                    if(nonConsumable.config != null)
                    nonConsumable.config.debugModePurchased = true;
                }
                ProcessValidPurchase(productId);
            }
        }
        #endregion

        #region Subscription specific
        bool prevSubscriptionActive;
        // Subscription timeout is approximate, so I need to compensate for variations. https://developer.android.com/google/play/billing/test
        void UpdateSubscriptinTimeout()
        {
            var currActive = subscriptionActive;
            if (!currActive && prevSubscriptionActive)
            {
                InitRuntimeProducts();
                currActive = subscriptionActive;
            }
            prevSubscriptionActive = currActive;
        }
        #endregion
    }

    public abstract partial class IAPManager<TSelf> : IAPManager
        where TSelf : IAPManager<TSelf>
    {
        new public static TSelf instance { get; private set; }
        protected override void Awake()
        {
            instance = (TSelf)this;
            base.Awake();
        }
    }
}
#endif
namespace FriendsGamesTools.IAP
{
    public enum SubscriptionPurchaseType { NewSubscriptionActivation, ExistingSubscriptionProlongation }
}