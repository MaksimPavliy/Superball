using FriendsGamesTools.DebugTools;

namespace Superball
{
    public class SuperballMoneyConfig : BalanceSettings<SuperballMoneyConfig>
    {
        public int levelWinMoneyBase = 50;
        public int levelWinMoneyCoef = 0;
        public float levelWinX3Chance => 0.25f;
    }
}