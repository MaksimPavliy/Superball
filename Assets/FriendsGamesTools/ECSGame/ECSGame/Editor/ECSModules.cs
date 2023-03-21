using FriendsGamesTools.ECSGame.DataMigration;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECSModuleFolder : ModulesFolder
    {
        public const string define = "ECSGame";
        public override string Define => define;
        public override bool canBeEnabled => true;
        public override List<string> dependFromPackages
            => base.dependFromPackages.Adding("com.unity.entities");
        public override List<string> dependFromModules 
            => base.dependFromModules.Adding(UIModule.define).Adding(GameRootModule.define);
        public override HowToModule HowTo() => new ECSGameHowTo();

#if ECSGame
        protected override void OnCompiledEnable()
        {
            base.OnCompiledEnable();
            SettingsInEditor<DataVersion>.EnsureExists();
        }
        protected override void OnCompiledGUI()
        {
            ShowIncVersionGUI();
            base.OnCompiledGUI();
        }
        //public override void ShortcutOnGUI()
        //{
        //    base.ShortcutOnGUI();
        //    ShowIncVersionGUI();
        //}

        static SaveMigrationModule migration => SaveMigrationModule.instance;
        public static void ShowIncVersionGUI()
        {
            EditorGUIUtils.InHorizontal(() =>
            {
                if (GUILayout.Button($"data version = {DataVersion.versionInd}, SAVE & INC data version {(migration.compiled ? "(Migration will be created...)" : "")}"))
                {
#if ECS_SAVE_MIGRATION
                    if (SaveMigrationModule.MessageIfNotOnMaster())
                        return;
#endif
                    var newVersion = DataVersion.versionInd + 1;
                    migration.OnVersionIncreased(DataVersion.versionInd, newVersion);
                    DataVersion.instance._versionInd = newVersion;
                    EditorUtils.SetDirty(DataVersion.instance);
                    AssetDatabase.SaveAssets();
                }
                if (GUILayout.Button("update curr data version", GUILayout.Width(180)))
                {
                    migration.OnVersionIncreased(DataVersion.versionInd - 1, DataVersion.versionInd);
                    AssetDatabase.SaveAssets();
                }
            });
            if (GUILayout.Button("Generate AOT now"))
                AOTForMobilesCodegen.Generate();
        }
#endif
                }
    public abstract class ECSModule : ModuleManager {
        public override string parentModule => ECSModuleFolder.define;
        public override List<string> dependFromModules =>
            base.dependFromModules.Adding(ECSModuleFolder.define).Adding(GameRootModule.define);
        public override bool hasDetailedView => true;
    }
}


