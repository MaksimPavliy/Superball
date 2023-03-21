using FriendsGamesTools.ECSGame.DataMigration;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class GameRootModule : ECSModule
    {
        public const string define = "ECS_GAMEROOT";
        public override string Define => define;
        public override List<string> dependFromModules
            => new List<string> { ECSModuleFolder.define };
        public override HowToModule HowTo() => new ECS_GAMEROOT_HowTo();
        protected override string debugViewPath => "ECSGame/ECS_GAMEROOT/Debug/GameRootDebugView";

#if ECS_GAMEROOT
        public override void ShortcutOnGUI()
        {
            base.ShortcutOnGUI();
            ShowDeleteSave();
        }
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            ShowDeleteSave();
            ShowOpenSave();
        }
        void ShowDeleteSave()
        {
            if (Serialization.saveExists && GUILayout.Button("Delete save"))
                Serialization.DeleteSave();
        }
        void ShowOpenSave()
        {
            if (Serialization.saveExists && GUILayout.Button("Open save"))
                EditorUtility.RevealInFinder(Serialization.path);
        }
#endif
    }
}


