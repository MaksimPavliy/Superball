using UnityEngine;

namespace FriendsGamesTools
{
    public class EnabledWhenDefine : MonoBehaviour
    {
        [SerializeField] string define;
        [SerializeField] GameObject parent;
        void Awake() => parent.SetActiveSafe(DefinesSettings.Exists(define));
    }
}