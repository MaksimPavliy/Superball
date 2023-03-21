#if ECS_LEVEL_BASED
using FriendsGamesTools.ECSGame.Locations;

namespace FriendsGamesTools.ECSGame
{
    public class LevelsView<TLocationView> : LocationsView<TLocationView, ChangeLocationsWindow>
        where TLocationView : LocationView
    {
        new public WinnableLocationsController controller => GameRoot.instance.Get<WinnableLocationsController>();
        Level.State prevState;
        protected override void Update()
        {
            base.Update();
            var currState = controller.state;
            if (prevState != currState && prevState == Level.State.playing)
            {
                if (currState == Level.State.win)
                    WinLevelWindow.Show();
                else if (currState == Level.State.lose)
                    LoseLevelWindow.Show();
            }
            prevState = currState;
        }
    }
}
#endif