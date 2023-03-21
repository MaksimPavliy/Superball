using System;
using UnityEngine;

namespace FriendsGamesTools.UI
{
    public abstract class Window : MonoBehaviour
    {
#if WINDOWS
        public static T Get<T>() where T : Window => Windows.Get<T>();
        public static T Show<T>(T prefab = null) where T: Window
        {
            var window = Windows.Get(prefab, true);
            window.shown = true;
            return window;
        }
        public static async void ShowWithDelay<T>(Func<T, float> getDelay) where T : Window
        {
            var window = Windows.Get<T>(null, true);
            var delay = getDelay(window);
            await Awaiters.SecondsRealtime(delay);
            window.shown = true;
        }
        public static void ShowWithDelay<T>(float delay) where T : Window
            => ShowWithDelay<T>(_ => delay);
        public bool closeOthers = true;
        public WindowOrientation orientation = WindowOrientation.Both;

        public virtual void OnClosePressed() => shown = false;
        public virtual bool shown
        {
            get => gameObject.activeSelf;
            set
            {
                if (value && closeOthers)
                    Windows.CloseAllBut(this);
                gameObject.SetActive(value);
                //Debug.Log($"window {name} {(value?"opened":"closed")}, time = {Time.time}");
            }
        }
#endif
    }
    public enum WindowOrientation { Both, Portrait, Landscape }
}