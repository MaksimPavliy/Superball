using FriendsGamesTools.ECSGame;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
#if ECS_SKIN_PROGRESS || ECS_SKINS
    public class ProgressSkinItemView : SkinItemView<ProgressSkin> {
        new ProgressSkinController controller => GameRoot.instance.Get<ProgressSkinController>();
#elif ECS_SKINS
    public class ProgressSkinItemView : SkinItemView {
        protected override SkinsController controller => throw new System.NotImplementedException();
#else
    public class ProgressSkinItemView : MonoBehaviour { 
#endif
    [SerializeField] Image icoFilled;
#if ECS_SKIN_PROGRESS || ECS_SKINS
        public override void UpdateView()
        {
            base.UpdateView();
            var progress = controller.GetProgress(skinInd);
            //var showLocked = controller.IsLocked(skinInd) && controller.skinIndToUnlock != skinInd;
            //unlockedParent.SetActive(!showLocked);
            //lockedParent.SetActive(showLocked);
            Show(ico, icoFilled, skinInd, progress);
        }
        public static void Show(Image ico, Image icoFilled, int skinInd, float progress)
        {
            var config = ProgressSkinsViewConfig.instance.items[skinInd];
            ico.SetSpriteSafe(config.ico);
            icoFilled.SetSpriteSafe(config.icoFilled);
            icoFilled.Safe(() => icoFilled.fillAmount = progress);
        }
#endif
    }
}
