namespace FriendsGamesTools.DebugTools
{
    public class DEBUG_LOGS_HowTo : HowToModule
    {
        protected override string docsURL => "https://docs.google.com/document/d/1qq9EbBXcsqjJcLKSAQk56oWeWACeVAl36rOmERetVR8/edit?usp=sharing";
        public override string forWhat => "showing and copying logs from mobile device";

        protected override void OnHowToGUI()
        {
            EditorGUIUtils.RichMultilineLabel("Enable this module in <b>DEBUG_PANEL</b>");
        }
    }
}