using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace FriendsGamesTools
{
    public class HapticSettingsDisableOnOldView : MonoBehaviour
    {
        [SerializeField] GameObject parent;
#if UNITY_IOS
        private void Awake()
        {
            if (Device.generation == DeviceGeneration.iPhoneSE1Gen || Device.generation == DeviceGeneration.iPhone6S || Device.generation == DeviceGeneration.iPhone6SPlus)
                parent.SetActiveSafe(false);
        }
#endif
    }
}
