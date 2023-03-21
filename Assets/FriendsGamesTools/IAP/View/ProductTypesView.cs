using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.IAP
{
    public class ProductTypesView : MonoBehaviour
    {
        [SerializeField] List<ProductType> shownTypes = new List<ProductType>();
        [SerializeField] PurchasableProductView prefab;
        [SerializeField] Transform parent;
#if IAP
        List<PurchasableProductView> shownItems = new List<PurchasableProductView>();
        private void Awake()
        {
            prefab.gameObject.SetActive(false);
        }
        private void Start()
        {
            UpdateView();
        }
        private void OnEnable() {

            UpdateView();
            IAPManager.onChanged += UpdateView;
        }
        private void OnDisable()
        {
            IAPManager.onChanged -= UpdateView;
        }
        void UpdateView()
        {
            if (IAPManager.instance == null) return;
            Utils.UpdatePrefabsList(shownItems, IAPSettings.instance.allProducts.Filter(p => shownTypes.Contains(p.GetProductType())),
            prefab, parent, (model, view) => view.Show(model));
        }
#endif
    }
}