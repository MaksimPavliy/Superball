#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HcUtils
{
    //Makes a copy of a component with all values
    public static class ComponentsCopy
    {
        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            var type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            var pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try { pinfo.SetValue(comp, pinfo.GetValue(other, null), null); }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            var finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
                finfo.SetValue(comp, finfo.GetValue(other));
            return comp as T;
        }
    }
}
#endif