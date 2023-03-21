using UnityEngine;

namespace FriendsGamesTools
{
    public abstract class MonoBehaviourHasInstance<TSelf> : MonoBehaviour
        where TSelf : MonoBehaviourHasInstance<TSelf>
    {
        public static TSelf instance { get; private set; }
        protected virtual void Awake() => instance = (TSelf)this;
        protected virtual void OnEnable() => instance = (TSelf)this; // For edit mode instances.
    }
}