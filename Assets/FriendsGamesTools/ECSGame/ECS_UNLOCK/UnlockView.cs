#if ECS_UNLOCK
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame.Upgradable
{
    public abstract class UnlockView : MonoBehaviour
    {
        public abstract Entity entity { get; }
        public abstract UnlockController controller { get; }
        [SerializeField] Button unlockButton;
        [SerializeField] GameObject availableParent;
        [SerializeField] GameObject notAvailableParent;
        [SerializeField] GameObject lockedParent;
        [SerializeField] GameObject unlockedParent;
        protected bool shownAvailable { get; private set; }
        protected bool shownLocked { get; private set; }
        protected virtual void Awake()
        {
            if (unlockButton!=null)
                unlockButton.onClick.AddListener(OnUnlockPressed);
        }
        protected virtual void Start()
        {
            UpdateView(entity.IsLocked(), controller.GetAvailable(entity));
        }
        protected virtual void Update()
        {
            var available = controller.GetAvailable(entity);
            var locked = entity.IsLocked();
            if (available != shownAvailable || locked != shownLocked)
                UpdateView(locked, available);
        }
        protected virtual void UpdateView(bool locked, bool available) {
            shownAvailable = available;
            shownLocked = locked;
            unlockButton.Safe(() => unlockButton.interactable = available);
            lockedParent.SetActiveSafe(locked);
            unlockedParent.SetActiveSafe(!locked);
            availableParent.SetActiveSafe(available);
            notAvailableParent.SetActiveSafe(!available);
        }
        protected virtual void OnUnlockPressed()
        {
            if (!controller.GetAvailable(entity))
                return;
            controller.Unlock(entity);
        }
    }
}
#endif