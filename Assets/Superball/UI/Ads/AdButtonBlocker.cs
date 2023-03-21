using UnityEngine;

namespace HcUtils
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AdButtonBlocker : MonoBehaviour
    {
        [SerializeField] private GameObject notAvailableParent;
        [SerializeField] private CanvasGroup canvasGroup;
        protected virtual bool RewardedAvailable => true;
        protected virtual bool GameLoaded => true;

        private void Awake()
        {
           if(!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        }

        void UpdateAvailable()
        {
            bool available = RewardedAvailable;
            if (canvasGroup) canvasGroup.interactable = available;
            if (notAvailableParent) notAvailableParent.gameObject.SetActive(!available);
        }

        private void Update()
        {
            if (!GameLoaded) return;
            
            UpdateAvailable();
        }
    }
}