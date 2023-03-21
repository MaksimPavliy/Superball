using UnityEngine;

namespace FriendsGamesTools
{
    public class DestroyOnAwake : MonoBehaviour
    {
        void Awake() => Destroy(gameObject);
    }
}