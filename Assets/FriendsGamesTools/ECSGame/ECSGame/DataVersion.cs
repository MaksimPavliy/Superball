#if ECSGame

namespace FriendsGamesTools.ECSGame
{
    public class DataVersion : SettingsScriptable<DataVersion>
    {
        public int _versionInd = 1;
        public static int versionInd => instance != null ? instance._versionInd : 0;
    }
}
#endif