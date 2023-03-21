using FriendsGamesTools;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class LevelBasedView : MonoBehaviourHasInstance<LevelBasedView>
    {
        [SerializeField] TextMeshProUGUI levelText;
#if ECS_LEVEL_BASED
        public static void SetLevelText(string text)
        {
            if (instance != null && instance.levelText!=null)
                instance.levelText.text = text;
        }
#endif
    }
}