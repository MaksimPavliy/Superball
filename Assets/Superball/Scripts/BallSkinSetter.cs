using FriendsGamesTools.ECSGame;
using HcUtils;

namespace Superball
{
    public class BallSkinSetter: CharacterSkinSetter
    {
        protected override object controller =>GameRoot.instance.Get<SuperballMoneySkinController>();
    }
}