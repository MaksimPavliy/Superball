using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class DEBUG_TOOLS_HowTo : HowToModule
    {
        public override string forWhat => "different tools for debugging";

        protected override void OnHowToGUI()
        {
            GUILayout.Label("Most of debug tools are in child modules");
            GUILayout.Space(10);
            DebugStopTime.ShowOnGUI("Put this to your scene. Press 'space' to stop/play time",
                "It just changes Time.timeScale");
        }

        ExampleScript DebugStopTime = new ExampleScript("DebugStopTime");
    }
}


