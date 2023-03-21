using TMPro;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class DebugPanelParamEditorLabel : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        public void Show(string text, int indent) => Show(text, indent, title);
        public static void Show(string text, int indent, TextMeshProUGUI label)
        {
            label.text = text;
            var m = label.margin;
            m.x = 5 + indent * 30;
            label.margin = m;
        }
    }
}
