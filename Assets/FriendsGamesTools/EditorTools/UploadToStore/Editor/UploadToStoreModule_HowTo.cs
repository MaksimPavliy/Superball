using UnityEngine;

namespace FriendsGamesTools.EditorTools.Upload
{
    public class UploadToStoreModule_HowTo : HowToModule
    {
        public override string forWhat => "upload build to store from cloud";
        UploadToStoreConfig config => SettingsInEditor<UploadToStoreConfig>.instance;
        protected override void OnHowToGUI() {
            EditorGUIUtils.RichMultilineLabel("set this to be Post-Build-Script path in Unity Cloud Build");
            var changed = false;
            EditorGUIUtils.Toolbar("Team", ref config.team, ref changed);
            GUILayout.BeginHorizontal();
            string path = config.team == UploadToStoreConfig.Team.FriendsGamesIncubator ?
                "Assets/FriendsGamesTools/EditorTools/UploadToStore/Editor/cloud_to_appstore_upload.sh" :
                "Assets/FriendsGamesTools/EditorTools/UploadToStore/Editor/cloud_to_appstore_upload_supersonic.sh";
            EditorGUIUtils.PushDisabling();
            EditorGUIUtils.TextField("", ref path, ref changed, labelWidth: 1);
            EditorGUIUtils.PopEnabling();
            if (GUILayout.Button("copy", GUILayout.Width(50))) {
                path.CopyToClipboard();
                Debug.Log("Path copied!");
            }
            GUILayout.EndHorizontal();
            if (changed)
                config.SetChanged();
        }
        protected override string docsURL => "https://docs.google.com/document/d/1f6EJ5Pin0ckeIM1hlCnR3tW3MDj4XhRCELzofIr1bzc/edit?usp=sharing";
    }
    public class UploadToStoreConfig : SettingsScriptable<UploadToStoreConfig>
    {
        public enum Team { FriendsGamesIncubator, SuperSonic }
        public Team team = Team.FriendsGamesIncubator;
    }
}