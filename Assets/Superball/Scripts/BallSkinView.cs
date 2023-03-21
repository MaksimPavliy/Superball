using FriendsGamesTools.ECSGame;
using HcUtils;
using UnityEngine;

namespace Superball
{
    public class BallSkinView : MonoBehaviour, ISkinSettable
    {
        public MoneySkinsViewConfig Config => MoneySkinsViewConfig.instance;
        [SerializeField] private SpriteRenderer _renderer;
        public void SetSkin(int ind)
        {
            _renderer.sprite=Config.items[ind].ico;
        }
    }
}