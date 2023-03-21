using FriendsGamesTools.DebugTools;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
    public class HardCurrencyDebugView : ECSModuleDebugPanel
    {
        public override string tab => "ECS";
        public override string module => "ECS_HARD_CURRENCY";
        [SerializeField] Button x10, div10;
#if ECS_HARD_CURRENCY
        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            x10.onClick.AddListener(OnX10Pressed);
            div10.onClick.AddListener(OnDiv10Pressed);
        }
        void OnX10Pressed() => GameRoot.instance.Get<HardCurrencyController>().DebugMultiply(10);
        void OnDiv10Pressed() => GameRoot.instance.Get<HardCurrencyController>().DebugMultiply(0.1f);
#endif
    }
}
