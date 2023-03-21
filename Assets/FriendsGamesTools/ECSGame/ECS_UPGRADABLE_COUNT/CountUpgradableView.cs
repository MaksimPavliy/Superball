#if ECS_UPGRADABLE_COUNT
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame.Upgradable
{
    public abstract class CountUpgradableView : MonoBehaviour
    {
        public abstract Entity entity { get; }
        public abstract CountUpgradableController controller { get; }
        public bool available => controller.GetAvailable(entity);

        [SerializeField] TextMeshProUGUI count;
        [SerializeField] GameObject maxCountParent;
        [SerializeField] GameObject notMaxCountParent;
        [SerializeField] GameObject noMoneyParent;
        [SerializeField] GameObject availableParent;
        [SerializeField] Button upgradeButton;
        [SerializeField] TextMeshProUGUI price;
        [SerializeField] bool dollarSign = true;
        protected virtual void Awake()
        {
            if (upgradeButton != null)
                upgradeButton.onClick.AddListener(OnUpgradePressed);
        }
        protected virtual void Update()
        {
            UpdateView();
        }
        protected virtual void UpdateView()
        {
            var upgradable = entity.GetComponentData<CountUpgradable>();
            var maxCount = controller.GetMaxCount(entity);
            var priceVal = controller.GetPrice(entity);
            bool hasPrice, noMoney;
#if ECS_PLAYER_MONEY
            hasPrice = controller.HasPrice(priceVal);
            noMoney = hasPrice && Player.Money.PlayerMoneyController.GetData().amount < priceVal;
#else
            hasPrice = noMoney = false;
#endif
            var available = controller.GetAvailable(entity, upgradable, maxCount, priceVal);
            if (count != null)
            {
                if (maxCount != -1)
                    count.text = $"{upgradable.count}/{maxCount}";
                else
                    count.text = $"{upgradable.count}";
            }
            var isMaxCount = upgradable.count >= maxCount && maxCount != -1;
            if (maxCountParent != null)
                maxCountParent.SetActive(isMaxCount);
            if (notMaxCountParent != null)
                notMaxCountParent.SetActive(!isMaxCount);
            if (availableParent != null)
                availableParent.SetActive(available);
            if (noMoneyParent != null)
                noMoneyParent.SetActive(noMoney);
            if (upgradeButton != null)
                upgradeButton.interactable = available;
            if (price != null && hasPrice)
                price.text = priceVal.ToShownMoney(dollarSign);
        }

        protected virtual void OnUpgradePressed()
        {
            controller.Upgrade(entity);
        }
    }
}
#endif