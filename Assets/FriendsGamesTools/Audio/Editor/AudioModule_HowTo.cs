using UnityEngine;

namespace FriendsGamesTools
{
    public class AudioModule_HowTo : HowToModule
    {
        public override string forWhat => "sounds and background music";
        protected override void OnHowToGUI()
        {
            MusicManager.ShowOnGUI("Put this to scene", 
                "Put AudioSource to set it to source property\n" +
                "Add some music to the list in inspector. It will be played looped\n" +
                "Use MusicManager.<b>on</b> to mute");
            SoundManager.ShowOnGUI("Put this to scene",
                "Call <b>Play(audioClip)</b> or <b>Play(audioClip, pos)</b> if sound should play only when its source visible\n" +
                "Use SoundManager.<b>on</b> to mute");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1RXMDsGAPSN1TVMAYhZYsJuG73YQRp74hwqPLYuEIQ6w/edit?usp=sharing";
        ExampleScript MusicManager = new ExampleScript("MusicManager");
        ExampleScript SoundManager = new ExampleScript("SoundManager");
    }
}


