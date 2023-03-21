namespace FriendsGamesTools
{
    public class TouchesModule_HowTo : HowToModule
    {
        public override string forWhat => "touches tracking and showing";
        protected override string docsURL => "https://docs.google.com/document/d/1YIFWmYf69gY8Thu72GJwSi0ODAOhMxaTj2U04arS2FU/edit?usp=sharing";
        protected override void OnHowToGUI()
        {
            TouchesView.ShowOnGUI("Drag this prefab to UI", 
                "Set <b>UICamera</b> in inspector\n" +
                "Set <b>showTouches</b> to make touches shown\n" +
                "If you need custom touch for example for tutorial, call <b>CreateTouch()</b>\n" +
                "\tCall <b>screenPos</b> or <b>targetWorldPos</b> to set position,\n" +
                "\tand <b>isTapping</b> to show animation");
        }
        ExamplePrefab TouchesView = new ExamplePrefab("TouchesView");
    }
}