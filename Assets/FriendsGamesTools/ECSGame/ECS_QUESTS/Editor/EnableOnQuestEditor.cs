#if QUESTS
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    [CustomEditor(typeof(EnableOnQuest))]
    public class EnableOnQuestEditor : Editor
    {
        EnableOnQuest data;
        MonoScript questClassToShow, questClassToUnlock;
        private void OnEnable()
        {
            data = (EnableOnQuest)target;
            questClassToShow = AssetByTypeAndName.Find<MonoScript>(data.showOnQuestTypeName, TypeFilterName.MonoScript, true);
            questClassToUnlock = AssetByTypeAndName.Find<MonoScript>(data.unlockOnQuestTypeName, TypeFilterName.MonoScript, true);
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var changed = false;
            ShowClass("Quest class to show", ref questClassToShow, ref data.showOnQuestTypeName);
            ShowClass("Quest class to unlock", ref questClassToUnlock, ref data.unlockOnQuestTypeName);
            if (changed)
                EditorUtils.SetDirty(data);

            void ShowClass(string title, ref MonoScript script, ref string questTypeName) {
                if (EditorGUIUtils.ObjectField(title, ref script, ref changed, -1, false))
                {
                    var type = ReflectionUtils.GetTypeByName(script?.name);
                    if (type != null && type.IsSubclassOf(typeof(Quest)))
                        questTypeName = script.name;
                    else
                    {
                        script = null;
                        questTypeName = string.Empty;
                    }
                }
            }
            
        }
    }
}
#endif