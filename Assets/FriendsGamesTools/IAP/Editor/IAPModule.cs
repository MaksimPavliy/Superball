using FriendsGamesTools.CodeGeneration;
using FriendsGamesTools.EditorTools.BuildModes;
using FriendsGamesTools.Integrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
#if IAP
using UnityEngine.Purchasing;
#endif

namespace FriendsGamesTools.IAP
{
    public class IAPModule : LibrarySetupManager<IAPModule>
    {
        public const string define = "IAP";
        public override string Define => define;
        public override string parentModule => FGTRootModule.define;
        public override HowToModule HowTo() => new IAPModule_HowTo();
        public override List<string> dependFromPackages => base.dependFromPackages.Adding("com.unity.purchasing");
        public override string SomeClassNameWithNamespace => "UnityEngine.Purchasing.IAPListener";
        protected override string debugViewPath => "IAP/Debug/IAPDebugView";

        //#region Coppa
        //enum COPPACompliance { COPPAUndefined, COPPACompliant, COPPANotCompliant } // == private enum UnityEditor.Connect.COPPACompliance
        //object unityConnect;
        //Type unitysCOPPAComplianceType;
        //void InitCoppaIfNeeded()
        //{
        //    if (unityConnect != null)
        //        return;
        //    var unityEditorAssembly = ReflectionUtils.GetAssemblyByName("UnityEditor");
        //    var unityConnectType = ReflectionUtils.GetTypeByName("UnityConnect", unityEditorAssembly);
        //    unityConnect = ReflectionUtils.GetStaticField(unityConnectType, "instance");
        //    unitysCOPPAComplianceType = ReflectionUtils.GetTypeByName("COPPACompliance", unityEditorAssembly);
        //}
        //COPPACompliance coppa
        //{
        //    get
        //    {
        //        InitCoppaIfNeeded();
        //        var projectInfo = ReflectionUtils.GetProperty(unityConnect, "projectInfo");
        //        var COPPA = ReflectionUtils.GetProperty(projectInfo, "COPPA");
        //        //var coppaLock = ReflectionUtils.GetProperty(projectInfo, "coppaLock");
        //        return (COPPACompliance)Enum.Parse(typeof(COPPACompliance), COPPA.ToString());
        //    }
        //    set
        //    {
        //        if (coppa == value)
        //            return;
        //        var unitysCoppa = Enum.Parse(unitysCOPPAComplianceType, value.ToString());
        //        ReflectionUtils.CallMethodExplicitParamTypes(unityConnect, "SetCOPPACompliance", (unitysCOPPAComplianceType, unitysCoppa));
        //    }
        //}
        //void ShowCOPPA()
        //{
        //    if (coppa == COPPACompliance.COPPAUndefined)
        //    {
        //        EditorGUIUtils.Error($"COPPA = <b>{coppa.ToString().Replace("COPPA", "")}</b>");
        //        EditorGUIUtils.InHorizontal(()=> {
        //            if (GUILayout.Button("set COPPA NotCompliant"))
        //                coppa = COPPACompliance.COPPANotCompliant;
        //            if (GUILayout.Button("set COPPA Compliant"))
        //                coppa = COPPACompliance.COPPACompliant;
        //        });
        //    }
        //}
        //#endregion

#if !IAP
        public override bool configured => false;
        protected override void OnNotCompiledGUI()
        {
            base.OnNotCompiledGUI();
            if (!UnityEditor.Purchasing.PurchasingSettings.enabled)
            {
                if (GUILayout.Button("enable purchasing"))
                    UnityEditor.Purchasing.PurchasingSettings.enabled = true;
            }
            else
            {
                //ShowCOPPA();
                GUILayout.Label("TODO: setup coppa");
                EditorGUIUtils.RichMultilineLabel("<b>UnityPurchasing</b> plugin not installed");
            }
        }
#else
        #region Unity config
        ProductCatalog unityConfig;
        void SaveToUnityConfig()
        {
            if (unityConfig == null)
                unityConfig = ProductCatalog.LoadDefaultCatalog();
            unityConfig.appleSKU = config.appleSKU;
            unityConfig.appleTeamID = config.appleTeamID;
            unityConfig.enableCodelessAutoInitialization = false;
            var oldProducts = unityConfig.allProducts.Clone();
            oldProducts.ForEach(p => unityConfig.Remove(p));
            config.consumables.ForEach(p => unityConfig.Add(CreateUnityProduct(p, UnityEngine.Purchasing.ProductType.Consumable)));
            config.nonConsumables.ForEach(p => unityConfig.Add(CreateUnityProduct(p, UnityEngine.Purchasing.ProductType.NonConsumable)));
            if (config.subscription.exists)
                config.subscription.products.ForEach(p => unityConfig.Add(CreateUnityProduct(p, UnityEngine.Purchasing.ProductType.Subscription)));
            File.WriteAllText(ProductCatalog.kCatalogPath, ProductCatalog.Serialize(unityConfig));
        }
        ProductCatalogItem CreateUnityProduct(AbstractProductSettings p, UnityEngine.Purchasing.ProductType type)
            => new ProductCatalogItem
            {
                applePriceTier = StorePriceTierUtils.ToApplePriceTier((decimal)p.price, p.GetProductTypeEditMode()),
                defaultDescription = new LocalizedProductDescription
                {
                    Title = p.title,
                    Description = p.description,
                    googleLocale = TranslationLocale.en_US
                },
                googlePrice = new Price { value = (decimal)p.price },
                id = p.productId,
                screenshotPath = p.appleReviewScreenshotPath,
                type = type
            };
        #endregion

        #region App id
        bool applicationIdsOk => ApplicationIdShouldBeValid.allOk;
        bool OnCheckAppIdGUI() {
            // Allow working here only while app ids set for all needed platforms.
            if (!applicationIdsOk)
            {
                EditorGUIUtils.WithColor(EditorGUIUtils.red,
                    () => EditorGUIUtils.RichMultilineLabel(
                        $"Application ids not set properly,\n" +
                        $"refer to {BuildModesModule.define} module for details\n" +
                        $"fix it to be able to work with IAP"));
                return false;
            }
            return true;
        }
        #endregion

        #region Export to platforms
        bool ios => BuildModeSettings.instance.IOSEnabled;
        bool android => BuildModeSettings.instance.AndroidEnabled;
        bool showExport = false;
        protected AppStoresCredentials credentials => SettingsInEditor<AppStoresCredentials>.instance;
        void ShowExportToPlatforms(ref bool changed)
        {
            var currChanged = changed;
            GUILayout.Space(20);
            EditorGUIUtils.InHorizontal(() =>
            {
                if (showExport)
                    EditorGUIUtils.ShowValid(ExportValid());
                EditorGUIUtils.ShowOpenClose(ref showExport);
                GUILayout.Label("export to stores");
            });
            if (!showExport)
                return;
            if (ios)
                AppleAppStoreExport.instance.OnGUI(ref currChanged);
            if (android)
                GooglePlayMarketExport.instance.OnGUI(ref currChanged);
            changed = currChanged;
        }
        bool ExportValid(StringBuilder sb = null)
        {
            var iosOK = true;
            if (ios)
                iosOK = AppleAppStoreExport.instance.ExportValid(sb);
            var androidOK = true;
            if (android)
                androidOK = GooglePlayMarketExport.instance.ExportValid(sb);
            return iosOK && androidOK;
        }
        #endregion

        #region Common
        public override string DoReleaseChecks()
        {
            InitIfNeeded();
            sb.Clear();
            IAPValid(sb);
            return sb.ToString();
        }
        public override bool configured {
            get {
                InitIfNeeded();
                return IAPValid();
            }
        }
        bool IAPValid(StringBuilder sb = null)
        {
            var valid = true;
            if (!UnityEditor.Purchasing.PurchasingSettings.enabled)
            {
                sb?.AppendLine($"Purchasing disabled at Unity->Services->In App Purchases");
                valid = false;
            }
            //if (!CoppaValid(sb))
            //    valid = false;
            if (!ReceiptValidationValid(sb))
                valid = false;
            if (!ProductsValid(sb))
                valid = false;
            if (!CodegenValid(sb))
                valid = false;
            return valid;
        }
        bool inited;
        void InitIfNeeded() {
            if (inited)
                return;
            inited = true;
            UpdateCodegen();
        }
        const string NewProductIdString = "NewProductId";
        IAPSettings config => SettingsInEditor<IAPSettings>.instance;
        protected override void OnCompiledEnable() {
            base.OnCompiledEnable();
            InitIfNeeded();
        }
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            GUILayout.Label("TODO: setup coppa"); //ShowCOPPA();
            if (!OnCheckAppIdGUI())
                return;
            var changed = false;
            OnLogsGUI(ref changed);
            OnProductsGUI(ref changed);
            ShowReceiptValidation(ref changed);
            ShowExportToPlatforms(ref changed); 
            if (changed) 
                Save();
        }
        public void Save()
        {
            // UpdateCodegen(); too expensive.
            EditorUtils.SetDirty(credentials);
            EditorUtils.SetDirty(config);
            SaveToUnityConfig();
        }
        #endregion

        #region Logs
        void OnLogsGUI(ref bool changed) => EditorGUIUtils.Toggle("logs", ref config.logs, ref changed);
        #endregion

        #region Restore iap exists
        bool IAPRestorePurchasesValid(StringBuilder sb = null) {
            if (BuildModeSettings.instance.IOSEnabled && !config.IOSRestorePurchasesButtonExists) {
                sb?.AppendLine($"No 'restore purchases' button found");
                return false;
            }
            return true;
        }
        void ShowRestoreIAPButtonExists(ref bool changed) {
            GUILayout.BeginHorizontal();
            EditorGUIUtils.ShowValid(IAPRestorePurchasesValid());
            EditorGUIUtils.Toggle("Is there a 'restore purchaes' button in the game?",
                ref config.IOSRestorePurchasesButtonExists, ref changed, labelWidth: 300);
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Products
        bool ProductsValid(StringBuilder sb = null)
        {
            var valid = true;
            if (!ProductsNotEmptyValid(sb))
                valid = false;
            if (!IdsUniquenessValid(sb))
                valid = false;
            config.consumables.ForEach(p=> {
                if (!ConsumableProductSettingsEditor.instance.ProductValid(p, sb))
                    valid = false;
            });
            config.nonConsumables.ForEach(p => {
                if (!NonConsumableProductSettingsEditor.instance.ProductValid(p, sb))
                    valid = false;
            });
            config.subscription.products.ForEach(p => {
                if (!SubscriptionProductSettingsEditor.instance.ProductValid(p, sb))
                    valid = false;
            });
            return valid;
        }
        void OnProductsGUI(ref bool changed)
        {
            ConsumableProductSettingsEditor.instance.ShowProductsList(ref changed);
            NonConsumableProductSettingsEditor.instance.ShowProductsList(ref changed);
            ShowSubscriptions(ref changed);
            ShowProductsValid();
            ShowRestoreIAPButtonExists(ref changed);
            ShowCodeGenerationForProducts();
            ShowDebugMode(ref changed);            
        }
        void ShowProductsValid()
        {
            sb.Clear();
            ProductsNotEmptyValid(sb);
            IdsUniquenessValid(sb);
            if (sb.Length > 0)
                EditorGUIUtils.InHorizontal(() =>
                {
                    EditorGUIUtils.ShowValid(false);
                    EditorGUIUtils.Error(sb.ToString());
                });
        }
        static StringBuilder sb = new StringBuilder();
        bool ProductsNotEmptyValid(StringBuilder sb = null)
        {
            if (config.allProducts.Count() == 0)
            {
                sb?.AppendLine("No IAP products added");
                return false;
            }
            return true;
        }
        bool IdsUniquenessValid(StringBuilder sb = null)
        {
            var unique = true;
            config.allProducts.ForEach(p => {
                var (ok, error) = config.CheckProductId(p.productId);
                if (!ok)
                {
                    sb?.AppendLine(error);
                    unique = false;
                }
            });
            return unique;
        }
        #endregion

        #region Subscriptions
        void ShowSubscriptions(ref bool changed)
        {
            sb.Clear();
            var currChanged = changed;
            EditorGUIUtils.InHorizontal(() =>
            {
                EditorGUIUtils.ShowValid(SubscriptionValid(sb));
                GUILayout.Label(config.subscription.exists ? "subscriptions enabled" : "subscriptions disabled");
                EditorGUIUtils.ToggleMadeFromButton("add subscription", "remove subscription", ref config.subscription.exists, ref currChanged);
                
            });
            if (!config.subscription.exists)
                return;
            EditorGUIUtils.Toggle("free trial?", ref config.subscription.freeTrialExists, ref currChanged);
            //if (showProductDetails)
            //{
                EditorGUIUtils.TextField("title", ref config.subscription.title, ref currChanged);
                EditorGUIUtils.TextField("description", ref config.subscription.description, ref currChanged);
            //}

            SubscriptionProductSettingsEditor.instance.ShowProductsList(ref currChanged);

            if (sb.Length > 0)
                EditorGUIUtils.Error(sb.ToString());
            changed = currChanged;
        }
        bool SubscriptionValid(StringBuilder sb = null) {
            if (!config.subscription.exists)
                return true;
            var valid1 = SubscriptionProductSettingsEditor.DescriptionValid(
                config.subscription.title, config.subscription.description, sb);
            var valid2 = config.subscription.products.Count > 0;
            if (!valid2)
                sb?.AppendLine($"no subscription products defined");
            var valid3 = true;
            foreach (var p in config.subscription.products)
            {
                if (!SubscriptionProductSettingsEditor.instance.ProductValid(p, null))
                    valid3 = false;
            }
            return valid1 && valid2 && valid3;
        }
        #endregion

        #region Code generation for products
        CodeGenerator codegen = new CodeGenerator();
        void UpdateCodegen()
        {
            // Place for enums.
            var nameSpaceName = "FriendsGamesTools.IAP";
            var folder = codegen.RequireFolder("IAP");
            var productTypes = folder.RequireFile("ProductTypes_Generated").RequireDefineWrapper("IAP");
            var nameSpaceForEnums = productTypes.RequireNameSpace(nameSpaceName);

            // IAPManager - place for methods.
            var IAPManagerCS = folder.RequireFile("IAPManager_Generated");
            var nameSpaceForClass = IAPManagerCS.RequireNameSpace(nameSpaceName);
            var IAPManager = nameSpaceForClass.RequireClass("IAPManager");
            IAPManager.visibility.RequirePublic().virtualization.RequireAbstract().partialization.RequirePartial();

            // Subscriptions.
            var typeParamName = "type";
            if (config.subscription.exists)
                IAPManager.RequireMethod("void", IAP.IAPManager.ApplySubscriptionPurchasedMethodName, ("SubscriptionPurchaseType", typeParamName)).visibility.RequireProtected().virtualization.RequireAbstract();

            // Consumables.
            const string OnPurchaseClickedBody = "OnPurchasePressed(AbstractProductSettings.GetProductId(type.ToString()))";
            var consumableProductParam = (IAP.IAPManager.ConsumableProductEnumName, typeParamName);
            var nonConsumableProductParam = (IAP.IAPManager.NonConsumableProductEnumName, typeParamName);
            if (config.consumables.Count > 0)
            {
                GenerateNonSubscriptions<ConsumableProductSettings, IAPConsumableData>(IAP.IAPManager.ConsumableProductEnumName,
                    config.consumables, IAP.IAPManager.ApplyConsumablePurchasedMethodName, consumableProductParam,
                    "OnPurchaseConsumableButtonPressed", "GetConsumableProductType");
            }

            // Non-consumables.
            if (config.nonConsumables.Count > 0)
            {
                GenerateNonSubscriptions<NonConsumableProductSettings, IAPNonConsumableData>(IAP.IAPManager.NonConsumableProductEnumName,
                    config.nonConsumables, IAP.IAPManager.ApplyNonConsumablePurchasedMethodName, nonConsumableProductParam,
                    "OnPurchaseNonConsumableButtonPressed", "GetNonConsumableProductType");
            }

            void GenerateNonSubscriptions<TSettings, TData>(string enumName, IEnumerable<AbstractProductSettings> configs, string ApplyPurchasedMethodName,
                (string paramTypeName, string paramName) productParam, string OnPurchaseButtonPressedMethodName, string GetProductTypeMethodName)
                where TSettings : AbstractProductSettings
                where TData : IAPProductData
            {
                var productsEnum = nameSpaceForEnums.RequireEnum(enumName);
                configs.ForEach(p => productsEnum.RequireValue(p.GetProductIdSuffix()));

                IAPManager.RequireMethod("void", ApplyPurchasedMethodName, productParam).visibility.RequireProtected().virtualization.RequireAbstract();

                var purchaseMethod = IAPManager.RequireMethod("void", OnPurchaseButtonPressedMethodName, productParam).visibility.RequireProtected().virtualization.RequireVirtual();
                purchaseMethod.RequireExpressionBody(OnPurchaseClickedBody);

                IAPManager.RequireMethod(enumName, GetProductTypeMethodName, (typeof(TSettings).Name, "product"))
                    .RequireExpressionBody($"System.Enum.TryParse<{enumName}>(product.productIdSuffix, out var type) ? " +
                    $"type : ({enumName})(-1)").visibility.RequirePublic();

                IAPManager.RequireMethod(typeof(TData).Name, "GetData", productParam).visibility.RequirePublic()
                    .RequireExpressionBody($"GetProductData(AbstractProductSettings.GetProductId(type.ToString())) as {typeof(TData).Name}");
            }

            codegenComplete = codegen.complete;
            if (codegenComplete)
                codegenHash = CalcCodegenHash();
        }
        
        bool codegenComplete;
        const string CodegenNotCompleted = "IAP Codegen not completed";
        bool CodegenValid(StringBuilder sb = null)
        {
            if (!codegenComplete)
            {
                sb?.AppendLine(CodegenNotCompleted);
                return false;
            }
            return true;
        }
        long codegenHash;
        long CalcCodegenHash()
        {
            long hash = config.subscription.exists.ToHash();
            config.allProducts.ForEach(p => hash = hash.ToHash(p.GetProductIdSuffix().ToHash()));
            return hash;
        }
        void UpdateCodegenStatus()
        {
            if (codegenComplete && codegenHash != CalcCodegenHash())
                codegenComplete = false;
        }
        void ShowCodeGenerationForProducts()
        {
            UpdateCodegenStatus();
            EditorGUIUtils.InHorizontal(() => {
                EditorGUIUtils.ShowValid(codegenComplete);
                if (!codegenComplete)
                    EditorGUIUtils.Error(CodegenNotCompleted);
                else
                    GUILayout.Label("Code generation complete");
            });
            if (codegenComplete)
                return;
            if (GUILayout.Button("Run codegeneration"))
                GenerateEnums();
        }
        void GenerateEnums() => codegen.Generate();
        #endregion

        #region Receipt validation
        // https://docs.unity3d.com/Manual/UnityIAPValidatingReceipts.html
        bool showReceiptDetails;
        bool obfuscationClassesExist => IAPManager.AppleTangleType != null && IAPManager.GooglePlayTangleType != null;
        bool ReceiptValidationValid(StringBuilder sb = null)
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            if (target != BuildTarget.Android && target!= BuildTarget.iOS)
            {
                sb?.AppendLine($"Cant check receipt validation, switch to android or ios build target");
                return false;
            }
            if (!obfuscationClassesExist)
            {
                sb?.AppendLine($"Receipt validation not generated");
                return false;
            }
            return true;
        }
        void ShowReceiptValidation(ref bool changed)
        {
            GUILayout.Space(20);
            sb.Clear();
            var valid = ReceiptValidationValid(sb);
            EditorGUIUtils.InHorizontal(() =>
            {
                EditorGUIUtils.ShowValid(valid);
                EditorGUIUtils.ShowOpenClose(ref showReceiptDetails);
                GUILayout.Label("receipt validation");
            });
            if (!showReceiptDetails)
                return;
            EditorGUIUtils.InHorizontal(() =>
            {
                if (valid)
                    GUILayout.Label("receipt validation classes generated");
                else
                    EditorGUIUtils.Error(sb.ToString());
                if (!valid)
                    showReceiptDetails = true;
            });
            if (!showReceiptDetails)
                return;
            if (GUILayout.Button("Generate receipt validation obfuscated classes"))
                EditorApplication.ExecuteMenuItem("Window/Unity IAP/Receipt Validation Obfuscator");
        }
        #endregion

        #region Debug mode
        bool showDebugMode; 
        void ShowDebugMode(ref bool changed)
        {
            GUILayout.Space(20);
            EditorGUIUtils.InHorizontal(() =>
            {
                EditorGUIUtils.ShowOpenClose(ref showDebugMode);
                GUILayout.Label("debugging");
            });
            if (!showDebugMode)
                return;
            EditorGUIUtils.Popup("mode in editor", ref config.defaultModeInEditor, ref changed);
            EditorGUIUtils.Popup("mode in build", ref config.defaultModeInBuild, ref changed);
            EditorGUIUtils.Popup("restore purchases mode", ref config.debugMode, ref changed);
            EditorGUIUtils.FloatField("subscription remaining time", ref config.debugSubscriptionRemainingOnAppLaunch, ref changed);
            EditorGUIUtils.Toggle("validate purchases", ref config.validatePurchases, ref changed);
        }
        #endregion
#endif
    }
}