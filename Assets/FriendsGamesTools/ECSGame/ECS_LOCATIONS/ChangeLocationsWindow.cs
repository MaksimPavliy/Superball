#if ECS_LOCATIONS
using FriendsGamesTools.UI;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame.Locations
{
    public abstract class ChangeLocationsWindow : Window
    {
        protected LocationsController controller => LocationsView.instance.controller;
        public static new void Show<T>() where T : ChangeLocationsWindow
            => Window.Show<T>().Init();
        [SerializeField] Button okButton;
        protected virtual void Awake()
        {
            if (okButton != null)
                okButton.onClick.AddListener(OnChangeLocationPressed);
        }
        protected virtual void Init()
        {
           
        }
        public virtual void OnChangeLocationPressed()
        {
            shown = false;
            controller.ChangeLocation();
        }
    }
}
#endif