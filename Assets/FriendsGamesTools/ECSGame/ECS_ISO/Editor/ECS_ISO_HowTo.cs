using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_ISO_HowTo : HowToModule
    {
        public override string forWhat => "tools for making isometric games";
        protected override void OnHowToGUI()
        {
            IsoCoos.ShowOnGUI("Put this script on some GameObject under Canvas you use to show isometric game, set x and y orts",
                "<b>WorldToIso(world)</b> and <b>IsoToWorld(iso)</b> transform between world and isometric position\n" +
                "<b>WorldToIsoDir(world)</b> and <b>IsoToWorldDir(iso)</b> transform between world and isometric position\n" +
                "<b>IsoDist(iso1, iso2)</b> - calculate distance in isometric space.Never use world space distances");
            GUILayout.Space(10);
        }
        protected override string docsURL => "https://drive.google.com/open?id=1NByQsVRTt0ChhYI61t0-zpK0KFzKFShwxEWeN2B_RvI";

        ExampleScript IsoCoos = new ExampleScript("IsoCoos");
    }
}