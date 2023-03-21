#if WINDOWS
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FriendsGamesTools.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class Windows : MonoBehaviourHasInstance<Windows>
    {
        List<Window> windows;
        [SerializeField] bool fillParentRect = true;
        protected override void Awake()
        {
            base.Awake();
            windows = transform.GetComponentsInChildren<Window>(true).ToList();
            if (fillParentRect)
                FillParentRect.Fill(GetComponent<RectTransform>());
        }
        public static Window Get(string name) => instance.windows.Find(w => w.name == name && OrientationOk(w));

        private static bool OrientationOk(Window w)
            => w.orientation == WindowOrientation.Both || w.orientation == screenOrientation;
        static bool portrait => Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown;
        static WindowOrientation screenOrientation => portrait ? WindowOrientation.Portrait : WindowOrientation.Landscape;

        public static T Get<T>(T prefab = null, bool inheritance = false) where T : Window
        {
            var window = instance.windows.Find(w =>
            {
                if (!OrientationOk(w))
                    return false;
                var type = w.GetType();
                var t = typeof(T);
                if (!inheritance)
                    return type == t;
                else
                    return t.IsAssignableFrom(type);
            }) as T;
            if (window == null && prefab != null)
            {
                window = Instantiate(prefab, instance.transform);
                instance.windows.Add(window);
            }
            return window;
        }
        public static bool anyShown => instance.windows.Any(w => w.shown);
        public static void CloseAll()
        {
            CloseAllBut(null);
        }
        public static void CloseAllBut(Window exception)
        {
            instance?.windows?.ForEach(w => {
                if (w.shown && w != exception)
                    w.shown = false;
            });
        }

        // Input blocked during game loops where windows were visible at any time.
        public static bool inputNotBlocked => nothingShownAfterLoop && !anyShown;
        public static bool nothingShownAfterLoop { get; private set; }
        protected virtual void LateUpdate()
        {
            nothingShownAfterLoop = !anyShown;
            ShowBack(backShown);
        }
        [SerializeField] GameObject backParent;
        protected virtual bool backShown => anyShown;
        protected virtual void ShowBack(bool show) => backParent.SetActiveSafe(show);
    }
}
#endif