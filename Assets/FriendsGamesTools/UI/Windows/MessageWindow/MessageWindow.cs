using TMPro;
using UnityEngine;

namespace FriendsGamesTools.UI
{
    public class MessageWindow : Window
    {
        [SerializeField] TextMeshProUGUI message;
#if WINDOWS
        public static void Show(string message, MessageWindow prefab = null)
        {
            if (prefab == null)
                prefab = Resources.Load<MessageWindow>("MessageWindow");
            var window = Windows.Get(prefab);
            window.message.text = message;
            window.shown = true;
        }
#endif
    }
}
