namespace FriendsGamesTools
{
    public class CameraModule_HowTo : HowToModule
    {
        public override string forWhat => "drag & pinch to move camera";
        protected override void OnHowToGUI()
        {
            CameraMover.ShowOnGUI("Derive from this script",
                "override <b>allowedMoveType</b> to define what movements are allowed - drag, scale or both. You can change that in runtime\n" +
                "Put derived script to camera and configure dragging/scaling speed, inertia in inspector\n");
            UIUnderPos.ShowOnGUI("To deny dragging over interfaces, check 'dont move above UI' and put this script to camera",
                "Set <b>rootOfWorld</b> in inspector to root canvas of what is considered draggable.");
            CameraMoverBounds.ShowOnGUI("To restrict movements to some visible space, put this script to the scene",
                "Configure it and set it to CameraMover's <b>bounds</b> field\n" +
                "Change it in runtime using <b>CameraMover.SetBounds(bounds)</b>");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1Me0iy24sm1sGXUKVnfAb-zRmOG9NNnMMWxDnGqZh7rc/edit?usp=sharing";

        ExampleScript CameraMover = new ExampleScript("CameraMover");
        ExampleScript UIUnderPos = new ExampleScript("UIUnderPos");
        ExampleScript CameraMoverBounds = new ExampleScript("CameraMoverBounds");
    }
}


