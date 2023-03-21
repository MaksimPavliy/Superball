using FriendsGamesTools.Integrations;
using System.Text;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class ManifestShouldNotBeSetAsDebuggable : ProjectSettingItem
    {
        public override string name => "MANIFEST_NOT_DEBUGGABLE";
        public override string description => "play market does not accept if manifest is set as debuggable";
        AndroidManifestManager manifest = new AndroidManifestManager();
        const string paramName = "android:debuggable";
        string debuggableString => $"{paramName}=\"true\"";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (manifest.contents.Contains(debuggableString))
                sb?.AppendLine($"android manifest contains {debuggableString}");
        }
        public override bool canSetup => true;
        protected override void Setup()
        {
            manifest.ReplaceParam("false", paramName, "manifest", "application");
            manifest.Save();
        }
    }
}