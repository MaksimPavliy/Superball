using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.UI
{
    public class YesNoWindow : Window
    {
        [SerializeField] TextMeshProUGUI question;
#if WINDOWS
        bool yes;
        public static async Task<bool> Show(string question, YesNoWindow prefab)
        {
            var window = Windows.Get(prefab);
            window.question.SetTextSafe(question);
            window.shown = true;
            while (window.shown)
                await Awaiters.EndOfFrame;
            return window.yes;
        }
        public void OnYesPressed()
        {
            yes = true;
            shown = false;
        }
        public void OnNoPressed()
        {
            yes = false;
            shown = false;
        }
#endif
    }
}
