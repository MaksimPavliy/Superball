using FriendsGamesTools.DebugTools;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    public class WindowsDebugView : ECSModuleDebugPanel
    {
        public override string tab => CommonTab;
        public override string module => "WINDOWS";
        [SerializeField] Button closeWindows;
#if WINDOWS
        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            closeWindows.onClick.AddListener(OnCloseAllPressed);
        }
        protected void OnCloseAllPressed() => Windows.CloseAll();
#endif
    }
}
