using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.IAP
{
    public class SubscriptionView : MonoBehaviour
    {
        [SerializeField] Toggle activeToggle;
        [SerializeField] GameObject activeParent;
        [SerializeField] GameObject notActiveParent;

        [SerializeField] Toggle availableToggle;
        [SerializeField] GameObject availableParent;
        [SerializeField] GameObject notAvailableParent;

        [SerializeField] TextMeshProUGUI remainingDuration;

        [SerializeField] Toggle freeAvailableToggle;
        [SerializeField] GameObject freeAvailableParent;
        [SerializeField] GameObject freeNotAvailableParent;
#if IAP
        protected virtual bool freeAvailable => false;

        private void OnEnable()
        {
            UpdateView();
            IAPManager.onChanged += UpdateView;
        }
        private void OnDisable()
        {
            IAPManager.onChanged -= UpdateView;
        }
        bool prevActive;
        private void Update()
        {
            var active = IAPManager.instance.subscriptionActive;
            if (prevActive && !active)
                UpdateView();
            prevActive = active;
            if (active)
                UpdateTime();
        }
        protected virtual void UpdateView()
        {
            var active = IAPManager.instance.subscriptionActive;
            if (activeToggle != null)
                activeToggle.isOn = active;
            if (activeParent != null)
                activeParent.SetActive(active);
            if (notActiveParent != null)
                notActiveParent.SetActive(!active);

            var available = IAPManager.instance.subscriptionAvailable;
            if (availableToggle != null)
                availableToggle.isOn = available;
            if (availableParent != null)
                availableParent.SetActive(available);
            if (notAvailableParent != null)
                notAvailableParent.SetActive(!available);

            var freeAvailable = this.freeAvailable;
            if (freeAvailableToggle != null)
                freeAvailableToggle.isOn = freeAvailable;
            if (freeAvailableParent != null)
                freeAvailableParent.SetActive(freeAvailable);
            if (freeNotAvailableParent != null)
                freeNotAvailableParent.SetActive(!freeAvailable);
            UpdateTime();
        }
        int shownRemainingSeconds;
        void UpdateTime()
        {
            if (remainingDuration == null)
                return;
            var seconds = (int)IAPManager.instance.subscriptionRemainingTime;
            if (shownRemainingSeconds == seconds)
                return;
            shownRemainingSeconds = seconds;
            remainingDuration.text = IAPManager.instance.subscriptionRemainingTime.ToShownTime();
        }
#endif
    }
}
