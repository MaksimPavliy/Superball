using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    public class MultiplierButtonView : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected AnimationClip deactivationClip;
        [SerializeField] protected TextMeshProUGUI multiplier;
        [SerializeField] protected Image remainingTimeFill;
#if UI
        protected const string StartAnimTrigger = "activate";
        protected const string FinishAnimTrigger = "deactivate";
        protected float deactivationDuraton => deactivationClip.length;
        protected bool shown { get; private set; }
        public virtual void UpdateView(bool shown, double multiplier, float elapsed, float duration)
        {
            var startShowing = !this.shown && shown;
            var finishShowing = this.shown && !shown;
            if (finishShowing)
            {
                hidingRemainingTime = deactivationDuraton;
                isHiding = true;
            }
            this.shown = shown;
            gameObject.SetActive(shown || isHiding);
            if (startShowing)
            {
                animator.SetTrigger(StartAnimTrigger);
                animator.Update(0);
            }
            if (finishShowing)
                animator.SetTrigger(FinishAnimTrigger);
            if (shown)
            {
                this.multiplier.SetTextSafe($"x{Mathf.RoundToInt((float)multiplier)}");
                remainingTimeFill.Safe(() => remainingTimeFill.fillAmount = elapsed / duration);
                isHiding = false;
            }
        }
        protected float hidingRemainingTime { get; private set; }
        protected bool isHiding { get; private set; }
        protected virtual void Update()
        {
            if (!isHiding)
                return;
            hidingRemainingTime -= Time.deltaTime;
            if (hidingRemainingTime > 0)
                return;
            gameObject.SetActive(false);
            isHiding = false;
        }
#endif
    }
}