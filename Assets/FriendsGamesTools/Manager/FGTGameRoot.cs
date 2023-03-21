using UnityEngine;

namespace FriendsGamesTools
{
    public class FGTGameRoot : MonoBehaviour
    {
        public static FGTGameRoot instance { get; private set; }
        protected virtual void Awake() {
            instance = this;
            DontDestroyOnLoad(gameObject);
            FrameRate.Init();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitOnStart() {
            if (instance != null) return;
            var go = new GameObject(typeof(FGTGameRoot).Name);
            go.AddComponent<FGTGameRoot>();
        }
    }
}
