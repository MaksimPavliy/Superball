#if ECS_LOCATIONS
using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame.Locations
{
    public abstract class ChangeLocationIco : MonoBehaviour
    {
        protected LocationsController controller => LocationsView.instance.controller;
        [SerializeField] GameObject canChangeParent;
        [SerializeField] protected DisablableLayoutElement canChangeInLayout;
        [SerializeField] TextMeshProUGUI currMultiplier;
        [SerializeField] TextMeshProUGUI nextMultiplier;
        [SerializeField] Button changeLocationButton;
        protected virtual void Start()
        {
            if (changeLocationButton != null)
                changeLocationButton.onClick.AddListener(OnChangeLocationPressed);
        }
        private void Update() => UpdateView();
        protected virtual void UpdateView()
        {
            var canChange = controller.canChange;
            if (canChangeParent!=null)
                canChangeParent.SetActive(canChange);
            if (canChangeInLayout != null)
                canChangeInLayout.SetShown(canChange);
            if (currMultiplier != null)
                currMultiplier.text = $"x{controller.multiplier.ToStringWithSuffixes()}";
            if (nextMultiplier != null)
            {
                if (controller.isMaxLocation)
                    nextMultiplier.text = "";
                else
                    nextMultiplier.text = $"x{controller.GetMultiplier(controller.currLocationInd + 1).ToStringWithSuffixes()}";
            }
            if (changeLocationButton != null)
                changeLocationButton.interactable = canChange;
        }
        public abstract void OnChangeLocationPressed();
    }
    public abstract class ChangeLocationIco<TChangeLocationWindow> : ChangeLocationIco
        where TChangeLocationWindow : ChangeLocationsWindow
    {
        public override void OnChangeLocationPressed() => ChangeLocationsWindow.Show<TChangeLocationWindow>();
    }
}
#endif