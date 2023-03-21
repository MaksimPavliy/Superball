
using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Superball
{
    public class SuperballRateUsWindow : Window
    {
        [SerializeField]
        private RateStar[] stars;

        [SerializeField]
        private Button rateButton;

        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private GameObject hintText;

        private int lastIndex = 0;

        private void Start()
        {
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].SetActive(false);
                stars[i].eSelected.AddListener(SetStarActive);
                stars[i].index = i;
            }

            rateButton.onClick.AddListener(Rate);
            closeButton.onClick.AddListener(CloseButtonPressed);
        }

        private void OnEnable()
        {
            hintText.gameObject.SetActive(false);
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].SetActive(false);
            }
        }
        private void CloseButtonPressed()
        {
            Close();
           
            //TODO put analytics here
            HCAnalyticsManager.OnRateUs(0);
        }

        private void Close()
        {
            GameRoot.instance.Get<SuperballRateApp>().IncrementShown();
            OnClosePressed();
        }
        public void Rate()
        {
            if(lastIndex ==0)
            {
                hintText.gameObject.SetActive(true);
                return;
            }
            if (lastIndex == 5)
            {
                RateApp.Open();
            }

            GameRoot.instance.Get<SuperballRateApp>().NeverShowAgain();
         
            //TODO put analytics here
            HCAnalyticsManager.OnRateUs(lastIndex);
            Close();
        }
        public void SetStarActive(int index)
        {

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].SetActive(i <= index);
            }
            lastIndex = index + 1;
        }

    }
}