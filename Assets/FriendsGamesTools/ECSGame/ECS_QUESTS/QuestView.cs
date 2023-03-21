#if QUESTS
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public class QuestView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title, description, progressLabel;
        [SerializeField] Image progress, ico;
        [SerializeField] GameObject completedParent, notCompletedParent;
        [SerializeField] Image rewardIco;
        [SerializeField] GameObject rewardParent;
        [SerializeField] TextMeshProUGUI rewardLabel;
        [SerializeField] Sprite rewardSoftSprite, rewardHardSprite;
        protected virtual void Update()
        {
            var quest = GameRoot.instance.Get<QuestsController>().currQuest;
            if (quest == null || !quest.visible) return;
            title.Safe(() => title.text = quest.viewConfig.title);
            description.Safe(() => description.text = quest.viewConfig.description);
            var maxItems = quest.maxQuestProgressItems;
            var currItems = Mathf.Min(quest.progressItems, maxItems);
            progressLabel.Safe(() => progressLabel.text = $"{currItems}/{maxItems}");
            progress.Safe(() => progress.fillAmount = (float)currItems / maxItems);
            var completed = currItems >= maxItems;
            completedParent.SetActiveSafe(completed);
            notCompletedParent.SetActiveSafe(!completed);
            ico.Safe(() => ico.sprite = quest.viewConfig.ico);

            if (quest.rewardMoney > 0)
                ShowReward(quest.rewardMoney, rewardSoftSprite);
            else if (quest.rewardHardCurrency > 0)
                ShowReward(quest.rewardHardCurrency, rewardHardSprite);
            else if (quest.viewConfig.customReward != null)
                ShowReward(1, quest.viewConfig.customReward);
            else
                rewardParent.SetActiveSafe(false);

            void ShowReward(double count, Sprite pic)
            {
                rewardParent.SetActiveSafe(true);
                rewardLabel.Safe(() => rewardLabel.text = count.ToShownMoney(dollarSign: false));
                rewardIco.Safe(() => rewardIco.sprite = pic);
            }
        }
    }
}
#endif