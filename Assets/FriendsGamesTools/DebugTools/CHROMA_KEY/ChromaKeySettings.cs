#if CHROMA_KEY
using System.Collections.Generic;

namespace FriendsGamesTools.DebugTools.ChromaKey
{
    public class ChromaKeySettings : SettingsScriptable<ChromaKeySettings>
    {
        public bool enabled = false;
        public List<string> disabledNames = new List<string>();
    }
}
#endif