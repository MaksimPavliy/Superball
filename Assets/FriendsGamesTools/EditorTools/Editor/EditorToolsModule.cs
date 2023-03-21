using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.EditorTools
{
    public class EditorToolsModule : ModulesFolder
    {
        public const string define = "EDITOR_TOOLS";
        public override string Define => define;
        public override bool canBeEnabled => true;
        public override HowToModule HowTo() => new EditorToolsModule_HowTo();
#if EDITOR_TOOLS
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            OnMissingScriptsGUI();
        }

        List<GameObject> withMissingScripts;
        private void OnMissingScriptsGUI()
        {
            if (Selection.gameObjects.Length == 0)
            {
                EditorGUIUtils.PushDisabling();
                GUILayout.Button("select gameobjects to find missing scripts in children");
                EditorGUIUtils.PopEnabling();
            }
            else
            {
                if (GUILayout.Button("find missing scripts in selection"))
                {
                    withMissingScripts = new List<GameObject>();
                    Selection.gameObjects.ForEach(go => FindMissingScripts.FindMissingScriptsRecursively(go, withMissingScripts));
                }
            }
            if (withMissingScripts != null)
            {
                if (withMissingScripts.Count > 0)
                {
                    GUILayout.Label($"{withMissingScripts.Count} gameobjects with missing scripts found:");
                    var _ = false;
                    EditorGUIUtils.PushDisabling();
                    withMissingScripts.ForEach(go => EditorGUIUtils.GameObjectField("   ", ref go, ref _));
                    EditorGUIUtils.PopEnabling();
                }
                else
                    GUILayout.Label("no missing scripts found");
                if (GUILayout.Button("clear"))
                    withMissingScripts = null;
            }
        }
#endif
    }
}