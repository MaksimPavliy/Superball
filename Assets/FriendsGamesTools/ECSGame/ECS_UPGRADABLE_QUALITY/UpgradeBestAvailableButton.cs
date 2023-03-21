#if ECS_UPGRADABLE_QUALITY || ECS_UPGRADABLE_COUNT
using System.Collections.Generic;
using FriendsGamesTools.UI;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame.Upgradable
{
    public abstract class UpgradeBestAvailableButton : MonoBehaviour
    {
        [SerializeField] Button activateBtn;
        [SerializeField] GameObject availableParent;
        [SerializeField] GameObject unavailableParent;
        [SerializeField] TweenShake tweenShake;
        private void Awake() => activateBtn.onClick.AddListener(OnActivateClicked);
        (UpgradableController c, Entity e, bool custom) GetWhatToUpgrade()
        {
            if (CustomUpgradeAvailable()) return (null, Entity.Null, true);

            var maxPrice = 0d;
            UpgradableController c = null;
            var e = Entity.Null;
            GameRoot.instance.controllers.ForEach(curr =>
            {
                var upgradable = curr as UpgradableController;
                if (upgradable == null) return;
                GetAvailableEntities(upgradable).ForEach(currE =>
                {
                    if (!upgradable.GetAvailable(currE) || !upgradable.HasPrice(currE)) return;
                    var currPrice = upgradable.GetPrice(currE);
                    if (currPrice <= maxPrice) return;
                    maxPrice = currPrice;
                    c = upgradable;
                    e = currE;
                });
            });
            return (c, e, false);
        }
        float updateInterval = 1f;
        float lastShownTime = -1;
        public void Show(bool show)
        {
            availableParent.SetActive(show);
            unavailableParent.SetActive(!show);
            if (show)
            {
                lastShownTime = Time.time;
                return;
            }
            lastShownTime = -1;
        }
        private void Update()
        {
            if (Time.time - lastShownTime > updateInterval)
            {
                var (c, _, custom) = GetWhatToUpgrade();
                bool shouldBeshown = custom || c != null;
                Show(shouldBeshown);
                tweenShake.SetEnabled(shouldBeshown);
            }
        }
        protected virtual void OnActivateClicked()
        {
            //if (RoomWindow.instance?.shown ?? false)
            //    await RoomWindow.instance.Closing();
            var (c, e, custom) = GetWhatToUpgrade();
            if (custom)
                OnCustomUpgradePressed();
            else
                OnUpgradePressed(c, e);
        }
        protected abstract IEnumerable<Entity> GetAvailableEntities(UpgradableController c);
        protected abstract void OnUpgradePressed(UpgradableController c, Entity e);
        protected virtual bool CustomUpgradeAvailable() => false;
        protected virtual void OnCustomUpgradePressed() { }
    }
}
#endif