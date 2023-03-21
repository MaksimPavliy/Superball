using UnityEngine;

namespace FriendsGamesTools
{
    public class LevelBasedModule_HowTo : HowToModule
    {
        public override string forWhat => "Level-based game, win-lose windows etc";
        protected override void OnHowToGUI()
        {
            EditorGUIUtils.RichMultilineLabel("if you turn on <b>ADS</b> module, rewards for ads will be proposed");
            CoreGameView.ShowOnGUI("put on scene and set inspector values");
            LevelsView.ShowOnGUI("put on scene and set inspector values",
                "create levels in prefabs, add <b>LevelView</b> script");
            WinLevelWindow.ShowOnGUI("put under windows on scene");
            LoseLevelWindow.ShowOnGUI("put under windows on scene");
            MainMenuWindow.ShowOnGUI("put under windows on scene");
            LocationsController.ShowOnGUI("override this script",
                "override <b>CheckWinLose()</b> and define when level is lost/won\n" +
                "override <b>levelWinMoney</b> and <b>levelWinX3Chance</b> to set parameters");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1HValUgZ0jM4TIi3saKQFTIQX4LdYEv8kbIwvgk9ujNQ/edit?usp=sharing";
        ExampleScript CoreGameView = new ExampleScript("CoreGameView");
        ExampleScript LevelsView = new ExampleScript("LevelsView");
        ExampleScript WinLevelWindow = new ExampleScript("WinLevelWindow");
        ExampleScript LoseLevelWindow = new ExampleScript("LoseLevelWindow");
        ExampleScript MainMenuWindow = new ExampleScript("MainMenuWindow");
        ExampleScript LocationsController = new ExampleScript("LocationsController");
    }
}