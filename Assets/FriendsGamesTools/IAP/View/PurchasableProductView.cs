using FriendsGamesTools.EditorTools.BuildModes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.IAP
{
    public class PurchasableProductView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title; 
        [SerializeField] TextMeshProUGUI description;
        [SerializeField] TextMeshProUGUI price;
        [SerializeField] Button purchase;
        [SerializeField] GameObject inProgress;
#if IAP
        protected virtual void Awake()
        {
            if (purchase != null)
                purchase.onClick.AddListener(OnPurchasePressed);
        }
        string productId;
        public virtual void Show(AbstractProductSettings product)
        {
            productId = product.productId;
            string title, description, currency;
            decimal price;
            var dataFromStore = IAPManager.instance.GetProductData(product.productId);
            if ((IAPDebugView.showLocalizedRealStoreDataInDebug || !BuildModeSettings.develop) && dataFromStore != null)
            {
                // Show localized data from store.
                title = dataFromStore.title;
                description = dataFromStore.description;
                price = dataFromStore.price;
                currency = dataFromStore.currency;
            }
            else
            {
                // Show debug data
                title = product.title;
                description = product.description;
                price = (decimal)product.price;
                currency = IAPManager.MockedCurrencyCode;
            }
            if (this.title != null)
                this.title.text = title;
            if (this.description  != null)
                this.description.text = description;
            if (this.price != null)
                this.price.text = ToShownPrice(price, currency);
        }
        public static string ToShownPrice(decimal price, string currency)
            => $"{((float)price).ToString(2)} {currency}";
        void ShowInProgress(bool inProgress)
        {
            if (this.inProgress != null)
                this.inProgress.SetActive(inProgress);
        }
        void OnPurchasePressed()
        {
            ShowInProgress(true);
            IAPManager.instance.OnPurchasePressed(productId, OnPurchaseUIResponse);
        }
        protected virtual void OnPurchaseUIResponse(bool success) => ShowInProgress(false);
#endif
    }
}
