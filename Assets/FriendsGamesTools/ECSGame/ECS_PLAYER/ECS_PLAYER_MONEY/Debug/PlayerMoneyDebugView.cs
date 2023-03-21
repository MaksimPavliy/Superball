using FriendsGamesTools.DebugTools;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class PlayerMoneyDebugView : ECSModuleDebugPanel
    {
        public override string tab => "ECS";
        public override string module => "ECS_PLAYER_MONEY";
        [SerializeField] Button x10, div10;
#if ECS_PLAYER_MONEY
        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            x10.onClick.AddListener(OnX10Pressed);
            div10.onClick.AddListener(OnDiv10Pressed);
        }
        void OnX10Pressed() => GameRoot.instance.Get<Player.Money.PlayerMoneyController>().DebugMultiply(10);
        void OnDiv10Pressed() => GameRoot.instance.Get<Player.Money.PlayerMoneyController>().DebugMultiply(0.1d);
#endif
    }
}
