#if QUESTS
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public class QuestsView : MonoBehaviourHasInstance<QuestsView>
    {
        [SerializeField] Highlighter highlighterPrefab;
        Highlighter highlighter;
        TutorialButton button;
        private void ClearArrow() {
            if (highlighter != null)
                Destroy(highlighter.gameObject);
            highlighter = null;
            button = null;
        }
        public static void ShowArrow(TutorialButton button, Highlighter.Side side)
        {
            if (instance == null || !GameRoot.instance.ViewEnabled) return;
            if (!Highlighter.shown) instance.ClearArrow();
            if (TutorialAssistantView.isBlocking) button = null;
            if (instance.button == button) return;
            instance.ClearArrow();
            if (button != null)
            {
                instance.button = button;
                instance.highlighter = instance.highlighterPrefab.CreateInstance();
                instance.highlighter.PressingButton(button, string.Empty, true, side).StartAsync();
            }
        }
        public static void HideArrow() => ShowArrow(null, Highlighter.Side.left);
    }
}
#endif