using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_ISO_HUMAN_HowTo : HowToModule
    {
        public override string forWhat => "ability to create different isometric 2d humans fast";
        protected override void OnHowToGUI()
        {
            OrientedHuman.ShowOnGUI("What animations will your human have? Derive from this class and declare your animation sprites",
                "Make it <b>[Serializable]</b>\n" +
                "declare <b>List<SpriteAndShadow></b> to have animation with shadow\n" +
                "declare <b>List<Sprite></b> if there are no shadows\n" +
                "declare <b>Sprite</b> if you need static pic");
            HumanView.ShowOnGUI("Derive your human view script from this",
                "<b>THuman</b> is a component you put to human entities\n" +
                "Override <b>Show()</b> to show any custom animations using <b>ShowAnimation()</b> calls");
            PicsSetup.ShowOnGUI("Optional: make a script to automatically setup human sprites in editor, derive it from this",
                "override <b>Setup()</b> to write settings sprites logic where\n" +
                "you can use <b>GetPathToFolder(sprite)</b> to get a folder by any sprite from it.\n" +
                "<b>GetSpritesFromFolder(folder)</b> gives you all animation sprites found in a folder\n" +
                "<b>GetSpritesShadowsFromFolder(folder)</b> gives all animation sprites and their shadows\n" +
                "<b>DoForEachOrientationFolder(..)</b> helps to setup all 4 orientations at once");
            GUILayout.Space(10);
        }
        protected override string docsURL => "https://drive.google.com/open?id=1I9tDgNj8EwzEaDD0jZPzDp2JSCJi3FnHJvRsYQa4Onc";

        ExampleScript OrientedHuman = new ExampleScript("OrientedHuman");
        ExampleScript HumanView = new ExampleScript("HumanView");
        ExampleScript PicsSetup = new ExampleScript("PicsSetup");
    }
}


