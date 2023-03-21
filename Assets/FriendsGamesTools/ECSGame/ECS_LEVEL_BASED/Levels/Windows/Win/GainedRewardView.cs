using System.Threading.Tasks;
using FriendsGamesTools;
using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class GainedRewardView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI rewardText;
        [SerializeField] TweenInTime tween;
        [SerializeField] float duration = 0.5f;
#if ECS_LEVEL_BASED
        public void SetReward(double reward) => rewardText.text = reward.ToStringWithSuffixes();
        public void PlayTween() => tween.SetEnabled(true);
        public async Task ShowingReward(double reward)
        {
            var tweening = tween.PlayingOnce();
            await AsyncUtils.SecondsWithProgress(duration, progress => SetReward(progress * reward), true);
            await tweening;
        }
#endif
    }
}