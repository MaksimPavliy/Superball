#if AUTO_BALANCE
using FriendsGamesTools.ECSGame;
using System.Linq;

namespace FriendsGamesTools.EditorTools.AutoBalance
{
    // Any player AI.
    public abstract class PlayerAI
    {
        protected GameRoot root => GameRoot.instance;
        public abstract void Update();
    }
}
#endif