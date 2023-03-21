using FriendsGamesTools.DebugTools;
using FriendsGamesTools.EditorTools.BuildModes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.IAP
{
    public class IAPDebugView : FGTModuleDebugPanel
    {
        public override string tab => "IAP";
        public override string module => "IAP";
        public override bool wholeTab => true;
        [SerializeField] TextMeshProUGUI purchasesAllowed;
        [SerializeField] TextMeshProUGUI realOrDebugAds;
        [SerializeField] Toggle mockedAdsToggle;
        [SerializeField] Toggle debugTypeRestorePurchases;
        [SerializeField] Toggle showLocalizedToggle;

#if IAP
        IAPManager iap => IAPManager.instance;

        [SerializeField] TextMeshProUGUI initingState;
        Color green = new Color(47f / 255f, 202f / 255f, 62f / 255f, 1);
        Color red = new Color(202f / 255f, 50f / 255f, 62f / 255f, 1);
        void UpdateInitedState()
        {
            if (iap.isIniting)
            {
                initingState.text = "initing...";
                initingState.color = Color.gray;
            }
            else if (iap.inited)
            {
                initingState.text = "inited";
                initingState.color = green;
            }
            else
            {
                initingState.text = "failed to init";
                initingState.color = red;
            }
        }

        void UpdateCanMakePayments()
        {
            if (iap.canMakePayments)
            {
                purchasesAllowed.text = "purchases allowed on device";
                purchasesAllowed.color = green;
            }
            else
            {
                purchasesAllowed.text = "purchases forbidden on device";
                purchasesAllowed.color = red;
            }
        }
        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            UpdateCanMakePayments();
            InitDebugAds();
        }
        protected override void UpdatePlaying()
        {
            base.UpdatePlaying();
            UpdateInitedState();
            UpdateDebugAds();
        }
        
        void InitDebugAds()
        {
            mockedAdsToggle.interactable = !BuildModeSettings.release;
            mockedAdsToggle.onValueChanged.AddListener(OnDebugChanged);
            debugTypeRestorePurchases.gameObject.SetActive(mockedAdsToggle.isOn);
            debugTypeRestorePurchases.onValueChanged.AddListener(OnDebugModeChanged);
            mockedAdsToggle.isOn = iap.mode == Mode.Mocked;
            debugTypeRestorePurchases.isOn = iap.config.debugMode == DebugRestorePurchasesMode.TestRestorePurchasesButton;
            if (showLocalizedToggle != null)
            {
                showLocalizedToggle.isOn = showLocalizedRealStoreDataInDebug;
                showLocalizedToggle.onValueChanged.AddListener(OnLocalizedChanged);
            }
        }
        void UpdateDebugAds()
        {
            if (iap.mode == Mode.Real)
            {
                purchasesAllowed.text = "store purchases";
                purchasesAllowed.color = green;
            }
            else
            {
                purchasesAllowed.text = "debug mocked purchases";
                purchasesAllowed.color = Color.magenta;
            }
        }
        public void OnDebugChanged(bool enabled)
        {
            if (BuildModeSettings.release) return;
            iap.SetMode(mockedAdsToggle.isOn ? Mode.Mocked : Mode.Real);
            debugTypeRestorePurchases.gameObject.SetActive(mockedAdsToggle.isOn);
        }
        public void OnDebugModeChanged(bool enabled)
        {
            if (BuildModeSettings.release) return;
            iap.config.debugMode = debugTypeRestorePurchases.isOn ? DebugRestorePurchasesMode.TestRestorePurchasesButton : DebugRestorePurchasesMode.TestTransactionsOnInit;
        }

        public static bool showLocalizedRealStoreDataInDebug = false;
        void OnLocalizedChanged(bool enabled)
        {
            showLocalizedRealStoreDataInDebug = enabled;
            IAPManager.instance.TriggerChanges();
        }
#endif
    }
}
