#if ECS_SKINS
using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using System.Collections.Generic;
using Unity.Entities;

namespace FriendsGamesTools.ECSGame
{
    public struct Skin : IBufferElementData
    {
        public bool locked;
    }

    public struct SkinsData : IComponentData
    {
        public int activeSkinInd;
    }

    // Some items that can be locked/unlocked, and one of them is active.
    public abstract class SkinsController : Controller
    {
        public abstract Entity entity { get; }
        public abstract IReadOnlyList<SkinViewConfig> viewConfigs { get; }
        public int unlockedSkinsCount => skins.Count(skin => !skin.locked);
        public bool anySkinLocked => entity.GetBuffer<Skin>().Any(skin => skin.locked);
        public int activeSkinInd => entity.GetComponentData<SkinsData>().activeSkinInd;
        public DynamicBuffer<Skin> skins => entity.GetBuffer<Skin>();
        public bool IsLocked(int ind) => skins[ind].locked;
        public int skinsCount => skins.Length;
        public override int updateEvery => 2;
        public override void InitDefault()
        {
            base.InitDefault();
            var e = entity;
            e.AddComponent(new SkinsData { activeSkinInd = 0 });
            var skins = e.AddBuffer<Skin>();
            viewConfigs.ForEach(skinConfig =>
                skins.Add(new Skin { locked = !skinConfig.startUnlocked }));
        }
        public virtual void ActivateSkin(int ind)
            => entity.ModifyComponent((ref SkinsData skins) => skins.activeSkinInd = ind );
        protected virtual void UnlockSkin(int ind)
        {
            skins.ModifyItem(ind, (ref Skin itemData) => itemData.locked = false);
            ActivateSkin(ind);
        }
    }

    public abstract class SkinsController<T> : SkinsController where T : struct, IComponentData
    {
        public override Entity entity => ECSUtils.GetSingleEntity<T>();
        public override void InitDefault()
        {
            ECSUtils.CreateEntity(new T());
            base.InitDefault();
        }
    }
}
#endif