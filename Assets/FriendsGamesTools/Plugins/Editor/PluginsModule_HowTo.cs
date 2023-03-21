using UnityEngine;

namespace FriendsGamesTools
{
    public class PluginsModule_HowTo : HowToModule
    {
        public override string forWhat => "some commonly used 3rd-party plugins";
        protected override void OnHowToGUI()
        {
            AsyncAwaitUtil.ShowOnGUI("<b>async/await</b> using coroutines, convenient but costs efficiency",
                "Additional awaiters:\n" +
                "   await Awaiters.<b>EndOfFrame</b>\n" +
                "   await Awaiters.<b>Seconds(0.5f)</b>\n" +
                "   await Awaiters.<b>SecondsRealtime(0.5f)</b>\n" +
                "   await Awaiters.<b>While(()=>true)</b>");
            //TapticManager.ShowOnGUI("Haptic feedback. So far only ios.",
            //    "just call <b>TapticManager.Impact(ImpactFeedback.Medium)</b>");
            Zip.ShowOnGUI("ZIP compressor for editor",
                "<b>Zip.CompressDirectory(dirName, outputPath)</b> and <b>Zip.UncompressZip(zipPath, outputDirectory)</b>");
            SimpleAnimation.ShowOnGUI("use this class if you need simple animation component, when you dont need animation controller",
                "Just call <b>Play()</b>, <b>Stop()</b> etc.");

            MiniJSON.ShowOnGUI("standard <b>MiniJSON</b> plugin",
                "Json.Deserialize(jsonString) as Dictionary<string, object>\n"+
                "str = Json.Serialize(dict)");
        }
        protected override string docsURL => "https://docs.google.com/document/d/1rCEJGz1CQ_JD82-QK2cDcO7kws-SW4TmwrnfEpH_rBI/edit?usp=sharing";
        ExampleFolder AsyncAwaitUtil = new ExampleFolder("AsyncAwaitUtil");
        //ExampleScript TapticManager = new ExampleScript("TapticManager");
        ExampleScript Zip = new ExampleScript("Zip");
        ExampleScript SimpleAnimation = new ExampleScript("SimpleAnimation");
        ExampleScript MiniJSON = new ExampleScript("MiniJSON");
    }
}


