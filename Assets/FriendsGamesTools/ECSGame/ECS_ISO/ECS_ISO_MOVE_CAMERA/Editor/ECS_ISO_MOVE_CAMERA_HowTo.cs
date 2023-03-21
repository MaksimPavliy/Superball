using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_ISO_MOVE_CAMERA_HowTo : HowToModule
    {
        public override string forWhat => "isometric camera movement - dragging, scale, borders";
        protected override void OnHowToGUI()
        {
            IsoCameraMover.ShowOnGUI("All the same as MoveCamera from <b>CAMERA</b> module");
            IsoCameraMoverBounds.ShowOnGUI("Except it has special isometric bounds, set with this script (like in CameraMoverBounds)",
                "<b>borders</b> is an <b>IsoRect</b> - isometric rectangle, so it can be used as camera borders. Works ok along with regular world space bounds.");
            IsoRect.ShowOnGUI("Use this script for camera isometric bounds rectangle",
                "Setup it just giving it 2 transforms as corners");
        }
        protected override string docsURL => "https://drive.google.com/open?id=1crUBDt5ZpVE8itjJfq3B0KCpgE_88Qnb61rRv-nAHvs";
        ExampleScript IsoCameraMover = new ExampleScript("IsoCameraMover");
        ExampleScript IsoCameraMoverBounds = new ExampleScript("IsoCameraMoverBounds");
        ExampleScript IsoRect = new ExampleScript("IsoRect");
    }
}


