using FriendsGamesTools.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using FriendsGamesTools;

namespace FriendsGamesTools
{
    public class DoYouLikeOurGameWindow : Window
    {
        [SerializeField] List<Button> starButtons;
        [SerializeField] List<GameObject> starChecked;
        [SerializeField] GameObject starsSelected;
        [SerializeField] Button okButton;
#if RATE_APP_ADVANCED

        public static void Show()
            => Show(RateAppAdvancedSettings.instance.doYouLikeWindow.prefab);
        RateAppAdvancedController controller => RateAppAdvancedController.instance;
        private void Awake()
        {
            starButtons.ForEachWithInd((button, ind) => button.onClick.AddListener(() => OnStarPressed(ind + 1)));
            starChecked.ForEachWithInd((star, ind) => star.SetActive(false));
            starsSelected.SetActiveSafe(false);
        }
        int selectedStarsCount = -1;
        void OnStarPressed(int count)
        {
            selectedStarsCount = count;
            starChecked.ForEachWithInd((star, ind) => star.SetActive(ind < count));
            starsSelected.SetActiveSafe(true);
        }
        public void OnRatePressed()
        {
            shown = false;
            if (selectedStarsCount == 5)
                controller.RateUs();
            else
                controller.DontRateUs();
        }
#endif
    }
}
