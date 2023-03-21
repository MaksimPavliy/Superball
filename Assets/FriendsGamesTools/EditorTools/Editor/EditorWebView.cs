#if UNITY_EDITOR && EDITOR_TOOLS
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FriendsGamesTools.EditorTools
{
    public static class EditorWebView
    {
        static Type FGTWebViewType;
        static PropertyInfo initialURLProperty, webViewProperty;
        static MethodInfo loadUrlMethod;
        private static void InitTypeIfNeeded()
        {
            if (FGTWebViewType != null)
                return;
            var unityEditorAssembly = ReflectionUtils.GetAssemblyByName("UnityEditor");
            var webViewType = ReflectionUtils.GetTypeByName("WebView", true);
            var parentType = ReflectionUtils.GetTypeByName("WebViewEditorWindowTabs", unityEditorAssembly);

            var assemblyName = new AssemblyName("UnityEditor");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            var typeBuilder = moduleBuilder.DefineType("FGTWebView",
                                  TypeAttributes.Public | TypeAttributes.Class, parentType);
            var methodBuilder = typeBuilder.DefineMethod("OnLocationChanged",
                MethodAttributes.Public | MethodAttributes.Virtual, null, new Type[] { typeof(string) });
            var urlParamBuilder = methodBuilder.DefineParameter(0,
                            ParameterAttributes.None,
                            "url");

            var methodIl = methodBuilder.GetILGenerator();
            methodIl.Emit(OpCodes.Ldarg_1);
            methodIl.Emit(OpCodes.Call, typeof(EditorWebView).GetMethod("OnURLChanged"));
            methodIl.Emit(OpCodes.Ret);

            FGTWebViewType = typeBuilder.CreateType();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            initialURLProperty = parentType.GetProperty("initialOpenUrl", flags);
            webViewProperty = parentType.GetProperty("webView", flags);
            loadUrlMethod = webViewType.GetMethod("LoadURL", flags);
        }
        public static void Show(string url = "https://www.google.com")
        {
            InitTypeIfNeeded();

            var window = ReflectionUtils.CallStaticGenericMethod(typeof(EditorWindow), "GetWindow", FGTWebViewType, new Type[] { });


            if (initialURLProperty.GetValue(window) == null)
                initialURLProperty.SetValue(window, url);
            else
            {
                var webView = webViewProperty.GetValue(window);
                loadUrlMethod.Invoke(webView, new object[] { url });

                //var webViewType = ReflectionUtils.GetTypeByName("WebView", true);
                //var devToolsMethod = webViewType.GetMethod("ShowDevTools", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                //devToolsMethod.Invoke(webView, new object[] { });
            }
        }

        public static void OnURLChanged(string url)
        {
            Debug.Log($"url changed to {url}");
            EditorWebView.url = url;
        }
        public static string url { get; private set; }
    }
}
#endif