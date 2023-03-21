#if ECS_PLAYER_MONEY
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Player.Money
{
    public class IncomeMultiplierView : MonoBehaviour
    {
        [SerializeField] GameObject activeParent;
        [SerializeField] TextMeshProUGUI multiplierLabel;
        [SerializeField] TextMeshProUGUI remainingTime;
        private void FixedUpdate()
        {
            var (multiplier, duration) = PlayerMoneyController.instance.GetIncomeMultiplierDuration();
            var active = duration < float.MaxValue;
            if (activeParent != null)
                activeParent.SetActive(active);
            if (!active)
                return;
            if (multiplierLabel != null)
                multiplierLabel.text = $"x{multiplier.ToString(0)}";
            if (remainingTime != null)
                remainingTime.text = $"({duration.ToShownTime()})";
        }
    }
}
#endif