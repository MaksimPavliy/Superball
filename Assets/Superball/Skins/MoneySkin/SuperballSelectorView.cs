using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Superball
{
    public class SuperballSelectorView:MonoBehaviourHasInstance<SuperballSelectorView>
    {
        public static List<SuperballSkinView> ActiveSkins = new List<SuperballSkinView>();
        public static void AddSkin(SuperballSkinView skin) => ActiveSkins.Add(skin);
        public void CleanUp() => ActiveSkins = ActiveSkins.Where(item => item != null).ToList();
        int lastSkinIndex = -1;
        private void Update()
        {
            var skinInd = GameRoot.instance.Get<SuperballMoneySkinController>().activeSkinInd;
            if (skinInd != lastSkinIndex)
            {
                lastSkinIndex = skinInd;
                UpdateSkins();
            }
        }
        public void UpdateSkins()
        {
            CleanUp();
            foreach (var skin in ActiveSkins)
            {
                skin.SetActiveSkin(GameRoot.instance.Get<SuperballMoneySkinController>().activeSkinInd);
            }
        }
    }
}