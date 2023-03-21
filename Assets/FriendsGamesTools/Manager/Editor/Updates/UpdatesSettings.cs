using System.Collections.Generic;

namespace FriendsGamesTools.ModulesUpdates
{
    public class UpdatesSettings : SettingsScriptable<UpdatesSettings>
    {
        public List<string> completedChanges = new List<string>();
    }
}