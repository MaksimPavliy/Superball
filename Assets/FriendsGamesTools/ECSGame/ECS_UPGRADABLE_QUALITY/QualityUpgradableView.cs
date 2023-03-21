#if ECS_UPGRADABLE_QUALITY
using FriendsGamesTools.Ads;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame.Upgradable
{
    public abstract class QualityUpgradableView : MonoBehaviour
    {
        public abstract Entity entity { get; }
        public abstract QualityUpgradableController controller { get; }
        public bool available => controller.GetAvailable(entity);

        [SerializeField] Slider levelProgressSlider;
        [SerializeField] TextMeshProUGUI level;
        [SerializeField] bool levelShownWithMax = false;
        [SerializeField] TextMeshProUGUI quality;
        [SerializeField] TextMeshProUGUI totalQuality;
        [SerializeField] GameObject maxLevelParent;
        [SerializeField] GameObject notMaxLevelParent;
        [SerializeField] GameObject noMoneyParent, enoughMoneyParent;
        [SerializeField] GameObject availableParent;
        [SerializeField] Button upgradeButton;
        [SerializeField] TextMeshProUGUI price;
        [SerializeField] bool dollarSign = true;
        GameRoot root => GameRoot.instance;
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
            var upgradable = entity.GetComponentData<QualityUpgradable>();
            var maxLevel = controller.GetMaxLevel(entity);
            var qualityPerLevel = controller.GetQualityPerLevel(entity);
            var priceVal = controller.GetPrice(entity);
            var available = controller.GetAvailable(entity, upgradable, maxLevel, qualityPerLevel, priceVal);
            bool hasPrice, noMoney;
#if ECS_PLAYER_MONEY
            hasPrice = controller.HasPrice(priceVal);
            noMoney = hasPrice && root.Get<Player.Money.PlayerMoneyController>().amount < priceVal;
#else
            hasPrice = noMoney = false;
#endif
            if (levelProgressSlider != null)
                levelProgressSlider.value = upgradable.quality / (float)qualityPerLevel;
            if (level != null)
            {
                if (!levelShownWithMax || maxLevel == -1)
                    level.text = upgradable.level.ToString();
                else
                    level.text = $"{upgradable.level}/{maxLevel}";
            }
            if (quality != null)
                quality.text = $"{upgradable.quality}/{qualityPerLevel}";
            if (totalQuality != null)
            {
                var str = $"{upgradable.quality + upgradable.level * qualityPerLevel}";
                if (maxLevel != -1)
                    str += $"/{qualityPerLevel * maxLevel}";
                totalQuality.text = str;
            }
            var isMaxLevel = maxLevel!=-1 && upgradable.level >= maxLevel;
            if (maxLevelParent != null)
                maxLevelParent.SetActive(isMaxLevel);
            if (notMaxLevelParent != null)
                notMaxLevelParent.SetActive(!isMaxLevel);
            if (noMoneyParent != null)
                noMoneyParent.SetActive(noMoney);
            if (enoughMoneyParent != null)
                enoughMoneyParent.SetActive(!noMoney);
            if (availableParent != null)
                availableParent.SetActive(available);
            if (upgradeButton != null)
                upgradeButton.interactable = available;
            if (price != null && controller.HasPrice(priceVal))
                price.text = priceVal.ToShownMoney(dollarSign);
        }

        protected virtual void OnUpgradePressed()
        {
#if ADS
            if (controller.AdsWhenNoMoney && !controller.EnoughMoney(entity)) {
                AdsManager.instance.rewarded.Show(Upgrade);
                return;
            }
#endif
            Upgrade();
        }
        void Upgrade() => controller.Upgrade(entity);
    }
}
#endif