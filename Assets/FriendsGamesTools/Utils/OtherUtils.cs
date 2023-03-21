using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FriendsGamesTools
{
    public static partial class Utils
    {
        public static void Stop(this Rigidbody body) {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
        public static bool IsVertical(this ScreenOrientation orient)
            => orient == ScreenOrientation.Portrait || orient == ScreenOrientation.PortraitUpsideDown;
        public static bool IsHorizontal(this ScreenOrientation orient)
            => orient == ScreenOrientation.Landscape
            || orient == ScreenOrientation.LandscapeLeft || orient == ScreenOrientation.LandscapeRight;
        public static bool EnumEquals<TEnum>(this TEnum a, TEnum b) where TEnum : Enum
            => EqualityComparer<TEnum>.Default.Equals(a, b);
#if UNITY_EDITOR
        public static void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
#endif
        public static void Swap<T>(ref T t1, ref T t2)
        {
            var temp = t1;
            t1 = t2;
            t2 = temp;
        }
        public static void Swap<T>(this IList<T> list, int ind1, int ind2)
        {
            var temp = list[ind1];
            list[ind1] = list[ind2];
            list[ind2] = temp;
        }
    }
    public delegate void ValRefAction<T1, T2>(T1 arg1, ref T2 arg2);
    public delegate void ValRefRefAction<T1, T2, T3>(T1 arg1, ref T2 arg2, ref T3 arg3);
    public delegate void RefAction<T1>(ref T1 arg1);
    public delegate void RefAction<T1, T2>(ref T1 arg1, ref T2 arg2);
    public delegate void RefAction<T1, T2, T3>(ref T1 arg1, ref T2 arg2, ref T3 arg3);
    public delegate void RefAction<T1, T2, T3, T4>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4);
}