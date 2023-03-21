using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_GAMEROOT_HowTo : HowToModule
    {
        public override string forWhat => "manage game data - save/load, controllers that know when game started, updated";
        protected override void OnHowToGUI()
        {
            gameRoot.ShowOnGUI("Derive your game root from this script and put it on scene",
                "use GameRoot.instance.Get<T>() to get controller instance\n" +
                "you can also define T field in your controller - it will be automatically filled with instance");
            controller.ShowOnGUI("Write game logic in controllers derived from this script",
                "override <b>InitDefault()</b> to create data for new player\n" +
                "override <b>OnInited()</b> to cache any data after any player data loading or creating\n" +
                "override <b>OnUpdate()</b> to do something each frame\n" +
                "you can also optimize game making some controllers less frequent\n" +
                "\tjust override <b>updateEvery</b> and use number bigger than 1\n" +
                "\tcontroller will be aware of how much time drained from previous update - <b>deltaTime</b>");
            GUILayout.Space(10);
        }
        protected override string docsURL => "https://drive.google.com/open?id=1UCxDkxofx0cJBpkkaEaNYIkWtuE9aG5fH2krqNd2YIA";

        ExampleScript gameRoot = new ExampleScript("GameRoot");
        ExampleScript controller = new ExampleScript("Controller");
    }
}


