using System.Collections.Generic;
using FriendsGamesTools.Share;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools
{
    public class CrossMarketingView : MonoBehaviour
    {
        public Button button;
        public TextMeshProUGUI header;
        public Image image;

#if CROSS_MARKETING
        private CrossMarketingAppData currentData;
        private CrossMarketingConfig dataSettings => CrossMarketingConfig.instance;
        private CrossMarketingShowSettings showSettings => CrossMarketingShowSettings.instance;

        private List<CrossMarketingAppData> appsToShow = new List<CrossMarketingAppData>();
        private string activeLink = "";

        private void Awake() {
            if (button == null) return;
            button.onClick.AddListener(OnButtonClick);
        }

        protected void OnEnable() => ShowRandomGame();
        protected void Start() {
            InitAppsList();
            ShowRandomGame();
        }

        public void InitAppsList() {
            appsToShow = new List<CrossMarketingAppData>();
            for (int i = 0; i < dataSettings.data.Count; i++) {
                var app = dataSettings.data[i];
                if (!app.available) continue;
                if (!showSettings.GetShown(app.id)) continue;
                appsToShow.Add(app);
            }
        }
        private void OnButtonClick() {
            if (activeLink != "") Application.OpenURL(activeLink);
        }
        public void ShowRandomGame() {
            if (appsToShow.Count == 0) return;
            currentData = appsToShow.RandomElement();
            if (currentData == null) {
                if (button != null) button.gameObject.SetActive(false);
                return;
            }
            if (button != null) button.gameObject.SetActive(true);
            if (header != null) header.text = currentData.appName;
            if (image != null) image.sprite = currentData.icon;
            if (TargetPlatformUtils.current == TargetPlatform.IOS)
                activeLink = ShareSettings.GetIOSGameLink(currentData.appName, currentData.appleID);
            else
                activeLink = ShareSettings.GetAndroidGameLink(currentData.androidPackageId);
        }
#endif
    }
}