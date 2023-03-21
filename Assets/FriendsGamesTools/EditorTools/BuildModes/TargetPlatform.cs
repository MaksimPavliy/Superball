using UnityEngine;

namespace FriendsGamesTools
{
    public enum TargetPlatform { IOS, Android }
    public static class TargetPlatformUtils
    {
        public static TargetPlatform current
        {
            get
            {
                bool isForAndroid = Application.platform == RuntimePlatform.Android;
#if UNITY_EDITOR
                isForAndroid = UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android;
#endif
                return isForAndroid ? TargetPlatform.Android : TargetPlatform.IOS;
            }
        }
#if UNITY_EDITOR
        public static UnityEditor.BuildTargetGroup currentBuildTargetGroup
        {
            get
            {
                switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
                {
                    default:
                    case UnityEditor.BuildTarget.Android: return UnityEditor.BuildTargetGroup.Android;
                    case UnityEditor.BuildTarget.iOS: return UnityEditor.BuildTargetGroup.iOS;
                }
            }
        }

        public static UnityEditor.BuildTarget ToBuildTarget(this TargetPlatform target) => target == TargetPlatform.Android ? UnityEditor.BuildTarget.Android : UnityEditor.BuildTarget.iOS;
        public static TargetPlatform ToTargetPlatform(this UnityEditor.BuildTargetGroup group) => group == UnityEditor.BuildTargetGroup.Android ? TargetPlatform.Android : TargetPlatform.IOS;

        public static bool IsInstalledInEditor(this TargetPlatform platform)
        {
            var target = platform.ToBuildTarget();
            var moduleManager = System.Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
            var isPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var getTargetStringFromBuildTarget = moduleManager.GetMethod("GetTargetStringFromBuildTarget", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            return (bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { target }) });
        }
#endif
        public static RuntimePlatform ToRuntimePlatform(this TargetPlatform type)
        {
            switch (type)
            {
                default:
                case TargetPlatform.IOS: return RuntimePlatform.IPhonePlayer;
                case TargetPlatform.Android: return RuntimePlatform.Android;
            }
        }
    }
}