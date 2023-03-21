#if ECS_BONUS
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using FriendsGamesTools.UI;
using FriendsGamesTools.Ads;
#if ADS
using FriendsGamesTools.Integrations;
#endif

namespace FriendsGamesTools.ECSGame.BonusEvent
{
    public abstract class BonusEventView<TBonus> : MonoBehaviour
        where TBonus : struct, IComponentData
    {
        [SerializeField] TextMeshProUGUI duration;
        [SerializeField] Image remainingForActivationFilledSprite;
        [SerializeField] bool reverseFill;
        [SerializeField] GameObject appearedParent;
        LayoutElement layoutElement;
        [SerializeField] Button activateButton;
        [SerializeField] Image remainingFilledSprite;
        [SerializeField] GameObject activeParent;
        [SerializeField] TextMeshProUGUI remainingDuration;
        protected virtual string GetDurationString() => $"for {(int)(controller.activatedDuration / 60)} min";
        protected BonusEventController<TBonus> controller { get; private set; }
        protected virtual void Awake()
        {
            controller = World.Active.GetExistingSystem<BonusEventController<TBonus>>();
            ShowState(BonusEvent.State.Hidden);
            if (activateButton != null)
                activateButton.onClick.AddListener(OnActivatePressed);
        }
        public BonusEvent.State shownState { get; private set; }
        protected virtual void ShowState(BonusEvent.State state) {
            shownState = state;
            DisablableLayoutElement.SetShown(
                state == BonusEvent.State.Appeared || state == BonusEvent.State.Active, gameObject, ref layoutElement);
            if (appearedParent != null)
                appearedParent.SetActive(state == BonusEvent.State.Appeared);
            if (activeParent != null)
                activeParent.SetActive(state == BonusEvent.State.Active);
        }
        protected virtual void Update()
        {
            var state = controller.state;
            if (state != shownState)
                ShowState(state);
            switch (state)
            {
                case BonusEvent.State.Appeared: UpdateAppeared(); break;
                case BonusEvent.State.Active: UpdateActive(); break;
            }
        }
        protected virtual void UpdateAppeared()
        {
            var progress = controller.GetStateProgress();
            ShowFill(progress, remainingForActivationFilledSprite, reverseFill);
            if (duration != null)
                duration.text = GetDurationString();
        }
        protected virtual void UpdateActive()
        {
            var progress = controller.GetStateProgress();
            ShowFill(progress, remainingFilledSprite, reverseFill);
            if (remainingDuration != null)
                remainingDuration.text = controller.data.remainingTime.ToShownTime();
        }
        static void ShowFill(float progress, Image filledSprite, bool reverseFill)
        {
            if (filledSprite == null)
                return;
            var fill = progress;
            if (reverseFill)
                fill = 1 - fill;
            filledSprite.fillAmount = fill;
        }
        public virtual void OnActivatePressed() => controller.Activate(false);
    }
}

#if ADS
namespace FriendsGamesTools.ECSGame.BonusEvent
{
    public abstract class BonusEventView<TBonus, TAds> : BonusEventView<TBonus>
        where TBonus : struct, IComponentData
        where TAds : AdsManager<TAds>
    {
        protected virtual bool useAds => true;
        public override void OnActivatePressed()
        {
            if (!controller.available)
                return;
            if (useAds)
            {
                controller.StartWatchingAd();
                AdsManager<TAds>.instance.rewarded.Show(success =>
                {
                    if (success)
                        base.OnActivatePressed();
                    else
                        controller.OnAdFailed();
                });
            } else
                base.OnActivatePressed();
        }
    }
}
#endif
#endif