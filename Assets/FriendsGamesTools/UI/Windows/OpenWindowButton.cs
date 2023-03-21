#if WINDOWS
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    [RequireComponent(typeof(Button))]
    public abstract class OpenWindowButton : MonoBehaviour
    {
        public string windowName;
        private void Awake()
            => GetComponent<Button>().onClick.AddListener(() => Windows.Get(windowName).shown = true);
    }
}
#endif