using UnityEngine;

namespace FriendsGamesTools
{
    public class CallOnEnable : MonoBehaviour
    {
        void OnEnable()
        {
            Debug.Log("hi");
        }
        private void OnDisable() {
            Debug.Log("bye");
        }
    }
}