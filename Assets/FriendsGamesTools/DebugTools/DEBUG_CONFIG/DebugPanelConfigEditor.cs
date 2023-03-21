using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public abstract class DebugPanelConfigEditor : MonoBehaviour
    {
        [SerializeField] ConfigEditorPrefabs prefabs;
        [SerializeField] Transform parent;
#if DEBUG_CONFIG
        public abstract BalanceSettings configInstance { get; }
        Type GetType(MemberInfo m) => m.GetFieldPropertyType();
        T GetValue<T>(MemberInfo m, int ind, object source)
        {
            if (ind == -1)
                return (T)m.GetValue(source);
            else
            {
                var listValue = (IList)m.GetValue(source);
                return (T)listValue[ind];
            }
        }
        void SetValue<T>(MemberInfo m, int ind, T value, object source)
        {
            if (ind == -1)
                m.SetValue(source, value);
            else
            {
                var listValue = (IList)m.GetValue(source);
                listValue[ind] = value;
            }
        }
        List<IDebugPanelParamEditor> editors = new List<IDebugPanelParamEditor>();
        void CreateParamEditor(object source, MemberInfo m, int ind, int indent)
        {
            var memberType = GetType(m);
            if (memberType.HasAttribute<NonSerializedAttribute>())
                return; // Allow hiding from config by declaring [NonSerialized].
            //Debug.Log($"{m.Name}: {type.Name}");
            var paramName = m.Name;
            var paramType = memberType;
            if (ind != -1)
            {
                paramName = $"{paramName}[{ind}]";
                paramType = memberType.GetGenericArguments()[0];
            }
            if (paramType == typeof(float))
                editors.Add(Instantiate(prefabs.floatPrefab, parent).Init(paramName, indent, ()
                    => GetValue<float>(m, ind, source), val => SetValue(m, ind, val, source)));
            else if (paramType == typeof(double))
                editors.Add(Instantiate(prefabs.doublePrefab, parent).Init(paramName, indent, ()
                    => GetValue<double>(m, ind, source), val => SetValue(m, ind, val, source)));
            else if (paramType == typeof(int))
                editors.Add(Instantiate(prefabs.intPrefab, parent).Init(paramName, indent, ()
                    => GetValue<int>(m, ind, source), val => SetValue(m, ind, val, source)));
            else if (paramType == typeof(bool))
                editors.Add(Instantiate(prefabs.boolPrefab, parent).Init(paramName, indent, ()
                    => GetValue<bool>(m, ind, source), val => SetValue(m, ind, val, source)));
            else if (paramType.IsEnum) {
                // Enums editing not implemented, just show curr value.
                var stringValue = GetValue<object>(m, ind, source).ToString();
                stringValue = $"{paramName} = {stringValue}";
                CreateLabel(stringValue, indent);
            } else if (paramType.GetInterfaces().Contains(typeof(IList))) {
                // Show list.
                var sourceList = (IList)m.GetValue(source);
                CreateLabel($"{paramName}({sourceList.Count})", indent);
                if (memberType.GetGenericArguments().Length > 0) {
                    var itemType = memberType.GetGenericArguments()[0];
                    for (int i = 0; i < sourceList.Count; i++) {
                        if (itemType.IsClass) {
                            CreateLabel($"{i}", indent + 1);
                            CreateParamEditorsForClassMembers(sourceList[i], indent + 2);
                        } else
                            CreateParamEditor(source, m, i, indent + 1);
                    }
                }
            } else if (paramType.HasAttribute<SerializableAttribute>() && memberType.IsClass) {
                // Show serializable class.
                CreateLabel(paramName, indent);
                var serializableClass = m.GetValue(source);
                CreateParamEditorsForClassMembers(serializableClass, indent + 1);
            }
        }
        void CreateLabel(string str, int indent) => Instantiate(prefabs.labelPrefab, parent).Show(str, indent);
        public void Show() => editors.ForEach(e => e.Show());
        protected virtual void Awake() => UpdateView();
        public virtual void UpdateView()
        {
            if (parent == null)
                parent = transform;
            parent.DestroyChildren();
            editors.Clear();
            if (configInstance == null)
                return;
            CreateParamEditorsForClassMembers(configInstance, 0);
            Show();
        }
        void CreateParamEditorsForClassMembers(object source, int indent)
        {
            var t = source.GetType();
            Type baseT = t.BaseType;
            while (baseT != null && baseT != typeof(BalanceSettings))
                baseT = baseT.BaseType;
            foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (baseT?.GetProperty(p.Name) != null)
                    continue;
                if (p.SetMethod == null || p.GetMethod == null)
                    continue;
                CreateParamEditor(source, p, -1, indent);
            }
            foreach (var p in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (baseT?.GetField(p.Name) != null)
                    continue;
                CreateParamEditor(source, p, -1, indent);
            }
        }
#endif
    }
}
