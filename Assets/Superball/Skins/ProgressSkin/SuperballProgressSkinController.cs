using FriendsGamesTools;
using FriendsGamesTools.ECSGame;

namespace Superball
{
    public class SuperballProgressSkinController : ProgressSkinController
    {
        SuperballProgressSkinConfig config => SuperballProgressSkinConfig.instance;
        protected override int GetPercentsPerLevel() => Utils.Random(config.percentsPerLevelMin, config.percentsPerLevelMax);
    }
}