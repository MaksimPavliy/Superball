using UnityEngine;

namespace FriendsGamesTools
{
    public abstract class HowToModule
    {
        public abstract string forWhat { get; }
        protected abstract void OnHowToGUI();
        protected virtual string docsURL => null;

        public void OnFullDocumentationGUI(string title = "show full documentation")
        {
            if (string.IsNullOrEmpty(docsURL))
                return;
            GUIUtils.URL(title, docsURL);
        }
        public void OnGUI()
        {
            EditorGUIUtils.RichMultilineLabel($"<b><i>{forWhat}</i></b>");
            GUILayout.Space(10);
            OnHowToGUI();
            OnFullDocumentationGUI();
            GUILayout.Space(10);
        }
    }
}