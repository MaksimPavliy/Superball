using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.UI;
using UnityEngine;
using UnityEngine.UI;
#if ECS_SKINS
using Unity.Entities;
#endif

namespace FriendsGamesTools.ECSGame
{
    public abstract class SkinItemView : MonoBehaviour
    {
        [SerializeField] protected Image ico;
        [SerializeField] Button activateButton;
        [SerializeField] GameObject selectionParent;
        [SerializeField] protected GameObject unlockedParent;
        [SerializeField] protected GameObject lockedParent;
        [SerializeField] ParticleSystem unlockParticles;
#if ECS_SKINS
        public SkinViewConfig config { get; private set; }
        public int skinInd { private set; get; } = -1;
        protected abstract SkinsController controller { get; }
        bool locked => controller.skins[skinInd].locked;
        bool selected => controller.activeSkinInd == skinInd;
        protected virtual void Awake() => activateButton.onClick.AddListener(ActivateSkin);
        public virtual void UpdateView()
        {
            ico.sprite = config.ico;
            selectionParent.SetActive(selected);
            unlockedParent.SetActive(!locked);
            lockedParent.SetActive(locked);
        }
        public void Show(SkinViewConfig config)
        {
            this.config = config;
            skinInd = controller.viewConfigs.IndexOf(config);
            UpdateView();
        }
        protected virtual void ActivateSkin()
        {
            controller.ActivateSkin(skinInd);
            Windows.Get<SkinsWindow>().UpdateView();
        }
        public void PlayUnlockEffect()
        {
            if (unlockParticles != null)
                unlockParticles.gameObject.SetActive(true);
        }
#endif
    }
#if ECS_SKINS
    public abstract class SkinItemView<T> : SkinItemView where T : struct, IComponentData
    {
        protected override SkinsController controller => GameRoot.instance.Get<SkinsController<T>>();
    }
#endif
}
