using System.Text;
using UnityEditor;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class AssetsShouldBeForcedText : ProjectSettingItem
    {
        public override string name => "FORCE_TEXT";
        const string error = "assets serialization mode should be 'forced text'";
        public override string description => error;
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (EditorSettings.serializationMode != SerializationMode.ForceText)
                sb.AppendLine(error);
        }
        public override bool canSetup => true;
        protected override void Setup()
        {
            if (EditorSettings.serializationMode == SerializationMode.ForceText)
                return;
            UnityEngine.Debug.Log("Re-serializing asset to make them 'force text'");
            EditorSettings.serializationMode = SerializationMode.ForceText;
            AssetDatabase.ForceReserializeAssets();
        }
    }
}