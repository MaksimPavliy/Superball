#if ECS_AB_TEST
using FriendsGamesTools.DebugTools;
using FriendsGamesTools.ECSGame;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.ABTests
{
    public class ABTestDebugPanel : ECSModuleDebugPanel
    {
        public override string module => "ECS_AB_TEST";
        public override string tab => "ECS";
        [SerializeField] TextMeshProUGUI selectedAB;
        protected override void OnEnablePlaying()
            => selectedAB.text = GameRoot.instance?.Get<ABTestsController>()?.eventNames.PrintCollection(", ")??"no tests controller";
    }
}
#endif