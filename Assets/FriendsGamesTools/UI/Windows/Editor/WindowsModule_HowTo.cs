using UnityEngine;

namespace FriendsGamesTools
{
    public class WindowsModule_HowTo : HowToModule
    {
        public override string forWhat => "simple windows functionality";
        protected override void OnHowToGUI()
        {
            Windows.ShowOnGUI("Put this script to UI canvas",
                "Use Windows.<b>Get</b><YourWindow>() to get YourWindow instance and show it.\n" +
                "Use Windows.<b>anyShown</b> Window.<b>CloseAll()</b> to operate all windows\n" +
                "MoveCamera wont move when window opened.");
            Window.ShowOnGUI("Derive your window script from this",
                "Use Window.<b>shown</b> to show/hide your window.\n" +
                "Use Window.<b>OnClosePressed()</b> to close window");
            OpenWindowButton.ShowOnGUI("Simple script to open window on button");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1xxTPzajpFWX0r2MOjUWl_BSCnR_wexRHEV9qpCFEnwE/edit?usp=sharing";
        ExampleScript Windows = new ExampleScript("Windows");
        ExampleScript Window = new ExampleScript("Window");
        ExampleScript OpenWindowButton = new ExampleScript("OpenWindowButton");
    }
}