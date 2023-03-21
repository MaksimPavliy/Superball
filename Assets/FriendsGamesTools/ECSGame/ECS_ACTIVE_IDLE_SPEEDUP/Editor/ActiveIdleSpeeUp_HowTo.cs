using UnityEngine;

namespace FriendsGamesTools.ECSGame.ActiveIdle
{
    public class ActiveIdleSpeeUp_HowTo : HowToModule
    {
        public override string forWhat => "active idle speedups";
        protected override void OnHowToGUI()
        {
            ActiveIdleSpeedUpController.ShowOnGUI(
                "derive from this script",
                "override <b>GetSpeedUpSettings()</b> to set <b>multiplier</b> and <b>duration</b> of speedup\n" +
                "optionally override <b>OnActivated(e, speedup)</b> or <b>OnDeactivated(e, speedup)</b> to do something when speedup activated\n" +
                "use e.HasSpeedUp() and e.GetSpeedUp() for custom usage\n");

            ActiveIdleSpeedUpView.ShowOnGUI(
                "use this script to show entity with active idle",
                "override <b>e</b> to select entity that is shown\n" +
                "override <b>controller</b> to select specific speedup\n" +
                "set <b>multiplierView</b> to show multiplier");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1tX3zeYIEso72AssMDyXreLgH3prRdjjfUd1eXltdfXM/edit?usp=sharing";

        ExampleScript ActiveIdleSpeedUpController = new ExampleScript("ActiveIdleSpeedUpController");
        ExampleScript ActiveIdleSpeedUpView = new ExampleScript("ActiveIdleSpeedUpView");
    }
}