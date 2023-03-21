using FriendsGamesTools.ModulesUpdates;
using UnityEngine;

namespace FriendsGamesTools
{
    public class FGTRootModule : ModuleManager
    {
        public const string define = "FriendsGamesTools";
        public override string parentModule => null;
        public override string Define => define;
        public override bool hasCollapsedView => false; 
        public override bool canBeEnabled => false;
        public override HowToModule HowTo() => new FGTRootModule_HowTo();

        public static FGTLocalSettings settings => SettingsInEditor<FGTLocalSettings>.instance;
        protected override void EnablingOnGUI(string toggleTitle)
        {
            GUILayout.BeginHorizontal();
            EditorGUIUtils.RichMultilineLabel("Enable modules you need and use them");
            if (GUILayout.Button("show wizard"))
                FGTWizard.Show();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            var changed = false;
            EditorGUIUtils.Toggle("Show all modules", ref settings.showAllModules, ref changed);
            ChangesEditorUI.ShowChangesEnabling();
            EditorGUIUtils.Toggle("alphabet order", ref settings.alphabetOrder, ref changed);
            GUILayout.EndHorizontal();
            if (changed)
            {
                EditorUtils.SetDirty(settings);
                FriendsGamesToolsWindow.instance.OnShowModulePressed(FriendsGamesToolsWindow.instance.shownParenModule);
            }
        }
    }
}