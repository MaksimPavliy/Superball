#if ECS_PLAYER_LEVEL
using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame.Player.Level
{
    public abstract class PlayerLevelView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI level;
        [SerializeField] string levelFormat;
        [SerializeField] Slider exp;
        [SerializeField] Image expFilledSprite;
        [SerializeField] GameObject levelupParent;
        [SerializeField] TweenInTime levelupEffect;
        private void Update()
        {
            var levelData = controller.data;
            var expForLevelUp = controller.GetExpForNextLevel(levelData.level);
            level.text = levelFormat.IsNullOrEmpty() ? levelData.level.ToString() : string.Format(levelFormat, levelData.level);
            var progress = (float)(levelData.exp / expForLevelUp);
            if (exp != null)
                exp.value = progress;
            if (expFilledSprite != null)
                expFilledSprite.fillAmount = progress;
            var levelup = controller.levelupAvailable;
            if (levelupParent != null)
                levelupParent.SetActive(levelup);
            if (levelupEffect != null)
                levelupEffect.SetEnabled(levelup);
            UpdateAutoPress(levelup);
        }
        protected PlayerLevelController controller => GameRoot.instance.Get<PlayerLevelController>();
        public virtual void OnLevelupTapped()
        {
            Windows.CloseAll();
            controller.ActivateLevelUp();
        }
        [SerializeField] float autopressAfterSec = -1;
        float remainingBeforeAutoPlay;
        bool prevAvailable;
        protected virtual bool autoLevelupAvailable => true;
        void UpdateAutoPress(bool available)
        {
            if (autopressAfterSec < 0)
                return;
            if (!prevAvailable) {
                if (available)
                    remainingBeforeAutoPlay = autopressAfterSec;
            } else {
                if (available) {
                    remainingBeforeAutoPlay -= Time.deltaTime;
                    if (remainingBeforeAutoPlay < 0 && autoLevelupAvailable) {
                        OnLevelupTapped();
                        remainingBeforeAutoPlay = autopressAfterSec;
                    }
                }
            }
            prevAvailable = available;
        }
    }
}
#endif