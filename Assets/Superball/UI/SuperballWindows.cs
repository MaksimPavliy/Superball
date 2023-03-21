using FriendsGamesTools.ECSGame;
using FriendsGamesTools.UI;

namespace Superball
{
    public class SuperballWindows : Windows
    {
        protected override bool backShown => base.backShown && !Windows.Get<MainMenuWindow>().shown;
    }
}