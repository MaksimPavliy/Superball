#if CROSS_MARKETING
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools
{
    [CustomEditor(typeof(CrossMarketingView))]
    public class CrossMarketingViewEditor : Editor
    {
        private CrossMarketingConfig settings => CrossMarketingConfig.instance;
        private CrossMarketingShowSettings showConfig => SettingsInEditor<CrossMarketingShowSettings>.instance;
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var data = settings.data;
            GUILayout.Space(10);
            EditorGUIUtils.LabelAtCenter("GAMES TO SHOW", FontStyle.Bold);
            GUILayout.Space(5);

            for (int i = 0; i < data.Count; i++) {
                if (i >= showConfig.data.Count) continue;
                if (!data[i].available) continue;
                GUILayout.BeginHorizontal();
                EditorGUIUtils.LabelAtCenter($"{data[i].appName}");
                GUILayout.EndHorizontal();
            }
        }
    }
}
#endif