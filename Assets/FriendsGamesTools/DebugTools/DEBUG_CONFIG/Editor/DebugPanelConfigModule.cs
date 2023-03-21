using UnityEditor.SceneManagement;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class DebugPanelConfigModule : DebugToolsModule
    {
        public const string define = "DEBUG_CONFIG";
        public override string Define => define;
        public override HowToModule HowTo() => new DEBUG_CONFIG_HowTo();
        protected override string debugViewPath => "DebugTools/DEBUG_CONFIG/ConfigDebugView";
#if DEBUG_CONFIG
        string configInputText = "";
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();

            bool changed = false;
            EditorGUIUtils.TextArea("paste config here", ref configInputText, ref changed, 300, 30);
            if (GUILayout.Button("Apply config to project"))
            {
                ConfigExportImport.Import(configInputText);
                EditorSceneManager.MarkAllScenesDirty();
                EditorSceneManager.SaveOpenScenes();
            }
            if (ConfigExportImport.saveExists)
            {
                if (GUILayout.Button("clear local saved configs"))
                    ConfigExportImport.ClearSave();
            }
            else GUILayout.Label("local saved config does not exist");

        }
#endif
    }
}