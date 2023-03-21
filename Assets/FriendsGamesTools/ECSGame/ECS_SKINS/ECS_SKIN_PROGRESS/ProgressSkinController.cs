#if ECS_SKIN_PROGRESS || ECS_SKINS
using System.Collections.Generic;
using FriendsGamesTools.ECSGame;
using Unity.Entities;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public struct ProgressSkin : IComponentData {
        public int percentsProgress;
        public int skinIndToUnlock;
    }
    public abstract class ProgressSkinController : SkinsController<ProgressSkin>
    {
        ProgressSkinsViewConfig viewConfig => ProgressSkinsViewConfig.instance;
        public override IReadOnlyList<SkinViewConfig> viewConfigs => ProgressSkinsViewConfig.instance.items;
        ProgressSkin data => entity.GetComponentData<ProgressSkin>();
        public int percents => data.percentsProgress;
        public float progress => Mathf.Clamp01(percents * 0.01f);
        public float GetProgress(int skinInd) => skinInd == skinIndToUnlock ? progress : (IsLocked(skinInd) ? 0 : 1);
        public int skinIndToUnlock => data.skinIndToUnlock;
        public virtual float adWinRewardMultiplier => 5; // x5 money for watching ad.
        public virtual int GetPercentsPerAd() => 0;
        protected abstract int GetPercentsPerLevel();
        public override void InitDefault()
        {
            base.InitDefault();
            entity.ModifyComponent((ref ProgressSkin p) => {
                p.percentsProgress = 0;
                p.skinIndToUnlock = GetNextSkinIndToUnlock(-1);
            });
        }
        public void GiveAdProgress() {
            var percents = Mathf.RoundToInt(GetPercentsPerLevel() * (adWinRewardMultiplier - 1) + GetPercentsPerAd());
            GiveProgress(percents);
        }
        public void GiveWinProgress() => GiveProgress(GetPercentsPerLevel());
        private void GiveProgress(int percents)
            => entity.ModifyComponent((ref ProgressSkin p) => p.percentsProgress += percents);
        public bool unlockAvailable => percents >= 100;
        public void UnlockOrLooseSkin(bool unlocked)
        {
            if (!unlockAvailable) return;
            if (unlocked)
                UnlockSkin(skinIndToUnlock);
            entity.ModifyComponent((ref ProgressSkin p) => {
                p.percentsProgress = 0;
                p.skinIndToUnlock = GetNextSkinIndToUnlock(p.skinIndToUnlock);
            });
        }
        int GetNextSkinIndToUnlock(int prevSkinIndToUnlock)
        {
            if (!anySkinLocked) return -1;
            var skinIndToUnlock = prevSkinIndToUnlock;

            do
            {
                skinIndToUnlock++;
                if (skinIndToUnlock >= skinsCount)
                    skinIndToUnlock = 0;
            } while (!IsLocked(skinIndToUnlock));
            return skinIndToUnlock;
        }
    }
}
#endif