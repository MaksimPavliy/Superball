using UnityEngine;

namespace FriendsGamesTools
{
    public abstract class MonoBehaviourSingleton<TSelf> : MonoBehaviour
        where TSelf : MonoBehaviourSingleton<TSelf>
    {
        static TSelf _instance;
        public static TSelf instance => EnsureExists();
        public static TSelf EnsureExists() => _instance ?? (_instance = new GameObject(typeof(TSelf).Name).AddComponent<TSelf>());
        protected virtual void Awake()
        {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }
            _instance = (TSelf)this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}