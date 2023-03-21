
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
#if ECS_SKIN_PROGRESS || ECS_SKINS
    public class ProgressSkinTabView : SkinsTabView<ProgressSkin> {
#elif ECS_SKINS
    public class ProgressSkinTabView : SkinsTabView {
        protected override SkinsController controller => throw new System.NotImplementedException();
#else
    public class ProgressSkinTabView : MonoBehaviour {
#endif

#if ECS_SKIN_PROGRESS || ECS_SKINS
        public override string TabName => "Backgrounds";
        public override string TabHint => "Unlock backgrounds by passing levels.";
#endif
    }
}
