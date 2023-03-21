using FriendsGamesTools.DebugTools;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class PlayerLevelDebugView : ECSModuleDebugPanel
    {
        public override string tab => "ECS";
        public override string module => "ECS_PLAYER_LEVEL";
        [SerializeField] Button addLevelButton;
#if ECS_PLAYER_LEVEL
        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            addLevelButton.onClick.AddListener(OnAddLevelPressed);
        }
        private void OnAddLevelPressed() => GameRoot.instance.Get<Player.Level.PlayerLevelController>().DebugAddLevel(1);
#endif
    }
}
