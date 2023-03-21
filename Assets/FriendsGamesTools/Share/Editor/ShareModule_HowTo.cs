using UnityEngine;

namespace FriendsGamesTools.Share
{
    public class ShareModule_HowTo : HowToModule
    {
        public override string forWhat => "'share' button";
        protected override void OnHowToGUI()
        {
            ShareManager.ShowOnGUI("",
                "call ShareManager.<b>Share(text)</b>\n" +
                "optionally you can give it <b>title</b> and <b>onResponse</b> parameters\n" +
                "You can test this module from debug panel");
            ShareButton.ShowOnGUI("You can put this script on your share button",
                "<b>title</b> - sharing title on androids\n" +
                "<b>text</b> - text to share\n" +
                "<b>addGameLink</b> - adds game link to shared text");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1kctAFmT1_wDIlPESsjjvFXqC5BpM9FZFMSOjbDqff3g/edit?usp=sharing";
        ExampleScript ShareManager = new ExampleScript("ShareManager");
        ExampleScript ShareButton = new ExampleScript("ShareButton");
    }
}