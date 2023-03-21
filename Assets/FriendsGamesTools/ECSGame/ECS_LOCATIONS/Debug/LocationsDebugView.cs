using FriendsGamesTools.DebugTools;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class LocationsDebugView : ECSModuleDebugPanel
    {
        public override string tab => "ECS";
        public override string module => "ECS_LOCATIONS";
        [SerializeField] Button buttonPrefab;
#if ECS_LOCATIONS
        List<Button> buttons;
        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            buttons = new List<Button>();
            var inds = new List<int>();
            for (int i = 0; i < controller.locationsCount; i++)
                inds.Add(i);
            buttons.Add(buttonPrefab);
            Utils.UpdatePrefabsList(buttons, inds, buttonPrefab, transform, (ind, button) =>
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = $"go to location {ind}";
                button.onClick.AddListener(()=> { controller.DebugChangeLocation(ind); UpdateView(); });
            });
        }
        protected override void OnEnablePlaying()
        {
            base.OnEnablePlaying();
            UpdateView();
        }
        void UpdateView()
        {
            for (int i = 0; i < controller.locationsCount; i++)
                buttons[i].interactable = controller.currLocationInd != i;
        }
        Locations.LocationsController controller => GameRoot.instance.Get<Locations.LocationsController>();
#endif
    }
}
