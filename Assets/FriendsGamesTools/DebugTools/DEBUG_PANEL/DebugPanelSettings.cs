using System.Collections.Generic;

namespace FriendsGamesTools.DebugTools
{
    public class DebugPanelSettings : SettingsScriptable<DebugPanelSettings>
    {
        public int openPanelPresses = 3;
        public float openPanelPressDuration = 2;
        public bool passwordIfNotDevelop = true;
        public int password = 2020;
        public bool startsShown = false;
        public List<string> disabledModules = new List<string>();
        public List<DebugPanelItemView> itemViews = new List<DebugPanelItemView>();
    }
}