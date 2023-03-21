using UnityEngine;

namespace FriendsGamesTools
{
    public class LogOnEnable : MonoBehaviour
    {
        private void OnEnable() => Debug.Log("OnEnable");
        private void OnDisable() => Debug.Log("OnDisable");
    }
}