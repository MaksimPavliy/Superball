using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FriendsGamesTools
{
    public static class ReflectionUtils
    {
        public static string FullNameInCode(this Type type) => type.FullName.Replace("+", ".");
        public static bool CanBeIdentifier(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            if (!char.IsLetter(str[0]) && str[0] != '_')
                return false;
            for (int i = 1; i < str.Length; ++i)
                if (!char.IsLetterOrDigit(str[i]) && str[i] != '_')
                    return false;
            return true;
        }

        public static FieldInfo GetFieldInfo(object obj, string name)
            => obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        public static T GetField<T>(object obj, string name)
        {
            var value = GetFieldInfo(obj, name).GetValue(obj);
            return (T)value;
        }
        public static void SetField<T>(object obj, string name, T value)
        {
            var field = GetFieldInfo(obj, name);
            field.SetValue(obj, value);
        }
        public static PropertyInfo GetIndexerInfo(this Type type, Type itemType)
            => type.GetProperty("Item", itemType, new Type[] { typeof(int) });
        public static PropertyInfo GetPropertyInfo(object obj, string name)
            => obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        public static T GetProperty<T>(object obj, string name)
        {
            var value = GetProperty(obj, name);
            return value == null ? default : (T)value;
        }
        public static object GetProperty(object obj, string name)
            => GetPropertyInfo(obj, name).GetValue(obj);
        public static void SetProperty<T>(object obj, string name, T value)
        {
            var prop = GetPropertyInfo(obj, name);
            prop.SetValue(obj, value);
        }
        public static FieldInfo GetStaticFieldInfo(Type type, string name)
            => (type == null || type == typeof(object)) ? null :
            (type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            ?? GetStaticFieldInfo(type.BaseType, name));
        public static PropertyInfo GetStaticPropertyInfo(Type type, string name)
            => type == null ? null :
            (type.GetProperty(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            ?? GetStaticPropertyInfo(type.BaseType, name));
        public static T GetStaticField<T>(this Type type, string name)
        {
            var value = GetStaticField(type, name);
            return value == null ? default : (T)value;
        }
        public static object GetStaticField(this Type type, string name, bool failIfNoField = true)
        {
            var field = GetStaticFieldInfo(type, name);
            if (field != null)
                return field.GetValue(null);
            var prop = GetStaticPropertyInfo(type, name);
            if (prop != null)
                return prop.GetValue(null);
            if (failIfNoField)
                throw new Exception($"{type.Name} has no static field or property with name {name}");
            return default;
        }
        public static void SetStaticField<T>(this Type type, string name, T value)
        {
            var field = GetStaticFieldInfo(type, name);
            field.SetValue(null, value);
        }

        public static bool CanCreateInstance(this Type type) => !type.IsAbstract && !type.ContainsGenericParameters && !type.IsInterface;
        public static object CreateInstance(Type type, params object[] args) => Activator.CreateInstance(type, args);
        public static T CreateInstance<T>(Type type, params object[] args) => (T)CreateInstance(type, args);
        public static Type GetTypeByName(string typeName, bool allAsemblies = false, bool fullName = false)
        {
            Type type = null;
            IterateTypes(currType => {
                if (currType.GetName(fullName) == typeName)
                    type = currType;
            }, allAsemblies);
            return type;
        }
        public static Type GetTypeByName(string typeName, Assembly assembly, bool fullName = false)
        {
            Type type = null;
            IterateTypes(currType =>
            {
                if (currType.GetName(fullName) == typeName)
                    type = currType;
            }, assembly);
            return type;
        }
        public static string GetName(this Type type, bool fullName) => fullName ? type.FullName : type.Name;
        public static void IterateTypes(Action<Type> action, bool allAsemblies = false)
        {
            if (allAsemblies)
                AppDomain.CurrentDomain.GetAssemblies().ForEach(assembly => IterateTypes(action, assembly));
            else
                IterateTypes(action, Assembly.GetExecutingAssembly()); 
        }
        public static void IterateTypes(Action<Type> action, Assembly assembly)
        {
            var types = assembly.GetTypes();
            foreach (var currType in types)
                action(currType);
            //foreach (var currTypeNoNesting in types)
            //IterateNestedTypesRecursively(currTypeNoNesting, action);
        }
        private static void IterateNestedTypesRecursively(Type type, Action<Type> action)
        {
            action(type);
            var nestedTypes = type.GetNestedTypes();
            nestedTypes.ForEach(nestedType => IterateNestedTypesRecursively(nestedType, action));
        }
        public static void ForEachDerivedType<T>(Action<Type> action)
            => ForEachDerivedType(typeof(T), action);
        public static void ForEachDerivedType(Type t, Action<Type> action)
        {
            var types = GetAllDerivedTypes(t);
            types.ForEach(action);
        }
        public static Assembly GetExecutingAssembly() => GetAssemblyByName("Assembly-CSharp");
        public static Assembly GetEditorAssembly() => GetAssemblyByName("Assembly-CSharp-Editor");
        public static Assembly GetFirstPassAssembly() => GetAssemblyByName("Assembly-CSharp-firstpass");
        public static Assembly GetEditorFirstPassAssembly() => GetAssemblyByName("Assembly-CSharp-Editor-firstpass");
        public static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().
                   SingleOrDefault(assembly => assembly.GetName().Name == name);
        }
        public static void ForEachUnityAssemblies(Action<Assembly> action)
        {
            void ActionIfAssemblyExists(Assembly assembly)
            {
                if (assembly != null)
                    action(assembly);
            }
#if UNITY_EDITOR
            ActionIfAssemblyExists(GetExecutingAssembly());
            ActionIfAssemblyExists(GetEditorAssembly());
            ActionIfAssemblyExists(GetFirstPassAssembly());
            ActionIfAssemblyExists(GetEditorFirstPassAssembly());
#else
            ActionIfAssemblyExists(Assembly.GetExecutingAssembly());
#endif
        }

        public static bool DoesClassExistInUnityAssembly(string classNameWithNamespace)
        {
            if (string.IsNullOrEmpty(classNameWithNamespace))
                return false;
            bool exists = false;
            ForEachUnityAssemblies(assembly =>
            {
                if (assembly?.GetType(classNameWithNamespace) != null)
                    exists = true;
            });
            return exists;
        }
        public static string GetMembersDescription(Type t)
        {
            bool MemberIgnored(MemberInfo m) => m.DeclaringType == typeof(Object);
            var sb = StringUtils.InitStringBuilder();
            sb.AppendLine($"{t.FullName} members:");
            var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var m in t.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (MemberIgnored(m))
                    continue;
                if (m.MemberType == MemberTypes.Method || m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field)
                    continue;
                sb.AppendLine($"member {m.Name} {m.MemberType} {m.GetType().Name}");
            }
            foreach (var m in t.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (MemberIgnored(m))
                    continue;
                sb.AppendLine(GetFieldDescription(m));
            }
            foreach (var m in properties)
            {
                if (MemberIgnored(m))
                    continue;
                sb.AppendLine(GetPropertyDescription(m));
            }
            foreach (var m in t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (MemberIgnored(m))
                    continue;
                var isPropertyMethod = properties.Any(p => p.GetMethod == m || p.SetMethod == m);
                if (isPropertyMethod)
                    continue;
                sb.AppendLine(GetMethodDescription(m));
            }
            if (t.IsEnum)
            {
                if (t.IsGenericType)
                    sb.AppendLine("enum is nested in generic type, getting its values not implemented");
                else
                {
                    foreach (var val in Enum.GetValues(t))
                        sb.AppendLine($"enum value {val}");
                }
            }
            return sb.ToString();
        }
        private static string Desc(bool propertyPresence, string propertyName) => propertyPresence ? propertyName : "";
        public static bool IsPrimitiveOrDecimal(this Type t) => t.IsPrimitive || t == typeof(decimal);
        public static bool IsStruct(this Type t) => t.IsValueType && !t.IsPrimitive && !t.IsEnum && t != typeof(decimal);
        public static string GetTypeDescription(Type t)
            => $"{Desc(t.IsPublic, "public ")}{Desc(t.IsNestedPublic, "nested public ")}{Desc(t.IsNotPublic,"private ")}" +
            $"{Desc(t.IsClass, "class ")}{Desc(t.IsInterface, "interface ")}{Desc(t.IsEnum, "enum ")}{Desc(t.IsStruct(), "struct ")}{GetNameWithBaseTypes(t)}" +
            $"\n{GetMembersDescription(t)}";

        private static string GetNameWithBaseTypes(Type t)
        {
            if (t == null || t == typeof(object))
                return "";
            return $"{t.FullName} : {GetNameWithBaseTypes(t.BaseType)}";
        }

        public static string GetAssemblyDescription(Assembly assembly, string searchInTypeNames = null, string searchInTypeDescriptions = null)
        {
            if (searchInTypeNames != null)
                searchInTypeNames = searchInTypeNames.ToLower();
            if (searchInTypeDescriptions != null)
                searchInTypeDescriptions = searchInTypeDescriptions.ToLower();
            var types = assembly.GetTypes();
            var sb = new StringBuilder($"Description of assembly {assembly.FullName} ({types.Length} types total):\n");
            foreach (var currType in types)
            {
                if (!string.IsNullOrEmpty(searchInTypeNames) && !currType.FullName.ToLower().Contains(searchInTypeNames))
                    continue;
                var typeDescription = GetTypeDescription(currType);
                if (!string.IsNullOrEmpty(searchInTypeDescriptions) && !typeDescription.ToLower().Contains(searchInTypeDescriptions))
                    continue;
                sb.AppendLine(typeDescription);
            }
            return sb.ToString();
        }
        public static string GetAllAssembliesDescription(string searchInTypeNames = null, string searchInTypeDescriptions = null)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var sb = new StringBuilder($"All assemblies({assemblies.Length}) description:\n");
            assemblies.ForEach(a => sb.AppendLine(GetAssemblyDescription(a, searchInTypeNames, searchInTypeDescriptions)));
            return sb.ToString();
        }
        static string GetVisibilityString(bool isPublic, bool isPrivate, bool isProtected, bool isInternal)
            => $"{Desc(isPublic, "public ")}{Desc(isPrivate, "private ")}{Desc(isProtected, "protected ")}{Desc(isInternal, "internal ")}";
        static string GetVisibilityString(MethodInfo m) => GetVisibilityString(m.IsPublic, m.IsPrivate, m.IsFamily, m.IsAssembly);
        static string GetVisibilityString(FieldInfo m) => GetVisibilityString(m.IsPublic, m.IsPrivate, m.IsFamily, m.IsAssembly);
        public static string GetMethodDescription(MethodInfo m)
            => $"method {GetVisibilityString(m)} {Desc(m.IsAbstract, "abstract ")}{Desc(m.IsVirtual,"virtual ")} {Desc(m.IsStatic, "static ")} {m.ReturnType.Name} " +
                $"{m.Name}({m.GetParameters().ConvertAll(p => $"{p.ParameterType.Name.ToLower()} {p.Name}").PrintCollection(",", "")}) " +
            $"{(m.ContainsGenericParameters?("has generic params"): "")}";
        public static string GetFieldDescription(FieldInfo m)
            => $"field {GetVisibilityString(m)}{Desc(m.IsStatic, "static ")}{m.FieldType.Name} {m.Name}";
        public static string GetPropertyDescription(PropertyInfo m)
            => $"property " +
            $"{(m.GetAccessors(true).Any(a => a.IsStatic) ? "static" : "")}" +
            $" {m.PropertyType} {m.Name} {{ {(m.CanRead ? $"{GetVisibilityString(m.GetMethod)}get;" : "")} {(m.CanWrite ? $"{GetVisibilityString(m.SetMethod)}set;" : "")} }}";

        public static bool DoesAssemblyExist(string name) => GetAssemblyByName(name) != null;
        public static List<Type> GetAllDerivedTypes<T>() => GetAllDerivedTypes(typeof(T));
        public static List<Type> GetAllDerivedTypes(Type baseClass)
        {
            var result = new List<Type>();
            ForEachUnityAssemblies(assembly => result.AddRange(GetAllDerivedTypes(baseClass, assembly)));
            return result;
        }
        public static List<Type> GetAllUnityAssemblyTypes()
        {
            var result = new List<Type>();
            ForEachUnityAssemblies(assembly => result.AddRange(assembly.GetTypes()));
            return result;
        }
        public static List<Type> GetAllDerivedTypes(Type baseClass, Assembly checkedAssembly)
        {
            var result = new List<Type>();
            IterateTypes(type =>
            {
                if (baseClass.IsAssignableFromAllowingGenerics(type) && baseClass != type)
                    result.Add(type);
            }, checkedAssembly);
            return result;
        }
        public static bool IsAssignableFromAllowingGenerics(this Type baseCanBeGenericType, Type givenType)
        {
            if (!baseCanBeGenericType.IsGenericType)
                return baseCanBeGenericType.IsAssignableFrom(givenType);
            var interfaceTypes = givenType.GetInterfaces();
            foreach (var it in interfaceTypes) {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == baseCanBeGenericType)
                    return true;
            }
            if (givenType == baseCanBeGenericType)
                return true;
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == baseCanBeGenericType)
                return true;
            Type baseType = givenType.BaseType;
            if (baseType == null)
                return false;
            return baseCanBeGenericType.IsAssignableFromAllowingGenerics(baseType);
        }
        public static bool DoesClassExist(string classNameWithNamespace) => GetTypeByName(classNameWithNamespace, allAsemblies: true, fullName: true) != null;
        public static bool TryGetAttribute<TAttribute>(this Type type, out TAttribute attribute)
            where TAttribute : Attribute
        {
            attribute = Attribute.GetCustomAttribute(type, typeof(TAttribute), true) as TAttribute;
            return attribute != null;
        }
        public static List<TAttribute> GetAllAttributeInstances<TAttribute>(this Type type)
            where TAttribute : Attribute
            => Attribute.GetCustomAttributes(type, typeof(TAttribute), true).ConvertAll(a => a as TAttribute);
        public static bool HasAttribute<TAttribute>(this MemberInfo m)
            where TAttribute : Attribute
            => Attribute.IsDefined(m, typeof(TAttribute));
        public static object GetValue(this MemberInfo m, object obj)
        {
            if (m is FieldInfo f)
                return f.GetValue(obj);
            else
                return (m as PropertyInfo).GetValue(obj);
        }
        public static void SetValue(this MemberInfo m, object obj, object value)
        {
            if (m is FieldInfo f)
                f.SetValue(obj, value);
            else
                (m as PropertyInfo).SetValue(obj, value);
        }
        public static Type GetFieldPropertyType(this MemberInfo m)
        {
            if (m is FieldInfo f)
                return f.FieldType;
            else if (m is PropertyInfo p)
                return p.PropertyType;
            else
                return null;
        }
        public static object CallStaticMethod(this Type t, string name, params object[] methodParams)
        {
            MethodInfo methodInfo;
            var currT = t;
            do
            {
                if (currT == null) throw new Exception($"static method {name} not found in {t.Name}");
                methodInfo = currT.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                currT = currT.BaseType;
            } while (methodInfo == null);
            return methodInfo.Invoke(null, methodParams);
        }
        public static object CallStaticGenericMethod(this Type t, string name, Type genericParam, Type[] types, params object[] methodParams)
        {
            var genericMethodInfo = t.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, types, null);
            var methodInfo = genericMethodInfo.MakeGenericMethod(genericParam);
            return methodInfo.Invoke(null, methodParams);
        }
        public static object CallMethod(object obj, string name, params object[] methodParams)
        {
            MethodInfo methodInfo = obj.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            return methodInfo.Invoke(methodInfo.IsStatic ? null : obj, methodParams);
        }
        public static void CallMethodExplicitParamTypes(object obj, string name, params (Type paramType, object paramValue)[] methodParams)
        {
            MethodInfo methodInfo = obj.GetType().GetMethod(name, methodParams.ConvertAll(p => p.paramType).ToArray());
            methodInfo.Invoke(methodInfo.IsStatic ? null : obj, methodParams.ConvertAll(p => p.paramValue).ToArray());
        }
        public static List<MemberInfo> GetMembersWithAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            var members = new List<MemberInfo>();
            foreach (var member in type.GetFields())
            {
                if (member.HasAttribute<TAttribute>())
                    members.Add(member);
            }
            foreach (var member in type.GetProperties())
            {
                if (member.HasAttribute<TAttribute>())
                    members.Add(member);
            }
            return members;
        }
        public static bool TryParseEnum(string value, Type enumType, out object parsedValue)
        {
            if (Enum.IsDefined(enumType, value))
            {
                parsedValue = Enum.Parse(enumType, value);
                return true;
            } else
            {
                parsedValue = null;
                return false;
            }
        }
    }
}