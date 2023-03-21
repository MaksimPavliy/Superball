#if HAPTIC
using System;
using FriendsGamesTools.Internal;
using TapticPlugin;
using UnityEngine;

namespace FriendsGamesTools
{
    public class Haptic
    {
        const string PrefsKey = "VibroOn";
        public static bool on
        {
            get => available && PlayerPrefsUtils.GetBool(PrefsKey, true);
            set => PlayerPrefsUtils.SetBool(PrefsKey, value);
        }
        static HapticSettings settings => HapticSettings.instance;
        public static bool available {
            get
            {
                if (Application.isEditor)
                    return settings.availableInEditor;
                else if (Application.platform == RuntimePlatform.Android)
                    return AndroidVibration.HasVibrator();
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return TapticManager.IsSupport();
                else
                    return false;
            }
        }
        public static void Vibrate() => Vibrate(settings.defaultType);
        public static void Vibrate(HapticType type)
        {
            if (settings.log)
                Debug.Log($"Haptic.Vibrate({type}) called (enabled={on}). Is haptic enabled: {SystemInfo.supportsVibration}");
            if (!on)
                return;

            if (Application.platform == RuntimePlatform.Android)
            {
                //if (!SystemInfo.supportsVibration) return;

                AndroidVibration.Vibrate(GetAndroidMS(type));
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //if (SpecialCaseForOldIPhone(type))
                    //return;
                TapticManager.Impact(type.ToIOSType());
            }
        }

        private static bool SpecialCaseForOldIPhone(HapticType type)
        {
#if UNITY_IOS
            if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneSE1Gen)
            {
                Handheld.Vibrate();
            }
            else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone6S || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone6SPlus)
            {
                switch (type)
                {
                    case HapticType.Light:
                    case HapticType.Medium:
                        BenoitFreslonVibration.Vibration.VibratePeek();
                        break;
                    case HapticType.Heavy:
                        BenoitFreslonVibration.Vibration.VibratePop();
                        break;
                }
            }
            else
            {
                return false;
            }

            return true;
#else
            return false;
#endif
        }

        private static long GetAndroidMS(HapticType type)
        {
            switch (type)
            {
                default:
                case HapticType.Heavy: return 60;
                case HapticType.Medium: return 50;
                case HapticType.Light: return 40;
            }
        }
    }
}
#endif