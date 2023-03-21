using FriendsGamesTools.DebugTools;
using UnityEngine.UI;
using FriendsGamesTools;

namespace FriendsGamesTools
{
    public class TouchesDebugView : FGTModuleDebugPanel
    {
        public override string module => "TOUCHES";
        public override string tab => CommonTab;
        public Toggle debugCursorToggle;

#if TOUCHES
        protected override void AwakePlaying()
        {
            base.AwakePlaying();
            if (TouchesView.instance == null)
                return;
            AddToggle(debugCursorToggle, () => TouchesView.instance.showTouches, val => TouchesView.instance.showTouches = val);
        }
#endif
    }
}