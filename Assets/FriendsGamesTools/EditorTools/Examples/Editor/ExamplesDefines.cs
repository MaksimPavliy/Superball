#if EXAMPLES
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace FriendsGamesTools.Examples
{
    public class ExamplesDefines
    {
        public static void InitOnLoad()
        {
            EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
        }

        static ExamplesSettings examples => SettingsInEditor<ExamplesSettings>.GetSettingsInstance(false);
        private static void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            if (examples == null) return;
            var currExample = examples.examples.Find(e => e.scene.SceneName == newScene.name);
            var added = new List<string>();
            var removed = new List<string>();
            examples.examples.ForEach(example=> {
                if (example.scene.SceneName == newScene.name)
                    added.Add(example.scene.SceneName);
                else
                    removed.Add(example.scene.SceneName);
            });
            // Trajectory3DExample
            DefinesModifier.ModifyDefines(added, removed);
            //Debug.Log($"new scene = {newScene.name}, is example = {currExample != null}");
        }
    }
}
#endif