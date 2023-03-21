using FriendsGamesTools.DebugTools;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class BonusDebugView : ECSModuleDebugPanel
    {
        public override string tab => "ECS";
        public override string module => "ECS_BONUS";
        [SerializeField] Button bonusButtonPrefab;

#if ECS_BONUS
        List<BonusEvent.BonusEventController> bonusControllers;
        List<Button> bonusButtons = new List<Button>();
        protected override void OnEnablePlaying()
        {
            bonusButtonPrefab.gameObject.SetActive(false);
            bonusControllers = GameRoot.instance?.controllers.ConvertAll(c
                => c as BonusEvent.BonusEventController).Filter(c => c != null);
            if (bonusControllers == null) return;
            Utils.UpdatePrefabsList(bonusButtons, bonusControllers, bonusButtonPrefab, transform, (c, button) =>
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = c.GetType().Name.Replace("Controller", "");
                button.onClick.AddListener(c.DebugAppear);
            });
        }
#endif
    }
}
