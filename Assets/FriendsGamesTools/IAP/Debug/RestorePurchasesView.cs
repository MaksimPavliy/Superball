using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.IAP
{
    public class RestorePurchasesView : MonoBehaviour
    {
        [SerializeField] Button button;
        [SerializeField] GameObject manualParent;
        [SerializeField] GameObject automaticParent;
#if IAP
        private void Awake()
        {
            if (manualParent != null)
                manualParent.SetActive(IAPManager.instance.manualRestorePurchases);
            if (automaticParent != null)
                automaticParent.SetActive(!IAPManager.instance.manualRestorePurchases);

            if (IAPManager.instance.manualRestorePurchases)
            {
                if (button != null)
                    button.onClick.AddListener(OnPressed);
            }
        }
        [SerializeField] GameObject isInProgressParent;
        void SetInProgress(bool inProgress)
        {
            if (isInProgressParent != null)
                isInProgressParent.SetActive(inProgress);
        }
        public void OnPressed()
        {
            SetInProgress(true);
            IAPManager.instance.OnRestorePurchasesClicked(success => SetInProgress(false));
        }
#endif
    }
}