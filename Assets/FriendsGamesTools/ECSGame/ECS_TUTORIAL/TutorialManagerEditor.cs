#if TUTORIAL && UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    [CustomEditor(typeof(TutorialManager))]
    public class TutorialManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("update chapters"))
            {
                var manager = (TutorialManager)target;
                manager.chapters = manager.transform.GetComponentsInChildren<TutorialChapter>().ToList();
            }
        }
    }
}
#endif