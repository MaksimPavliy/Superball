using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class ConfigEditorFromMonoBeh : DebugPanelConfigEditor
    {
        [SerializeField] BalanceSettings config;
#if DEBUG_CONFIG
        public override BalanceSettings configInstance => config;
        public void SetConfig(BalanceSettings config)
        {
            this.config = config;
            UpdateView();
        }
#endif
    }
}