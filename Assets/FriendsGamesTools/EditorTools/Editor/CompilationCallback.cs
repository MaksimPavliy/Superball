using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;

namespace FriendsGamesTools
{
    [ExecuteAlways]
    public class CompilationCallback : SettingsScriptable<CompilationCallback>
    {
        protected override bool inResources => false;
        protected override bool inRepository => false;
        
        public List<string> callbacks;
        public static void CallStaticMethodAfterRecompilation(Action doAfterRecompile)
        {
            if (instance==null || instance.callbacks == null)
                instance.callbacks = new List<string>();
            instance.callbacks.Add(doAfterRecompile.Method.EncodeToString());
            ForceRecompilation();
        }
        public static void ForceRecompilation()
        {
            var cMonoScript = MonoImporter.GetAllRuntimeMonoScripts()[0];
            MonoImporter.SetExecutionOrder(cMonoScript, MonoImporter.GetExecutionOrder(cMonoScript));
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {            
            if (instance == null || instance.callbacks == null)
                return;
            var callbacksCopy = instance.callbacks.ConvertAll(item=>item);
            instance.callbacks.Clear();
            foreach (var methodInfoString in callbacksCopy)
            {
                var methodInfo = SerializationUtils.DecodeFromString<MethodInfo>(methodInfoString);
                methodInfo.Invoke(null, null);
            }
        }

        #region Log compilation time
        static new CompilationCallback instance => SettingsInEditor<CompilationCallback>.instance;
        public static void InitOnLoad()
        {
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            CompilationPipeline.compilationFinished += OnCompilationFinished;
        }
        [SerializeField] bool logCompilationDuration;
        [SerializeField, HideInInspector] float startCompilingTime;
        private static void OnCompilationStarted(object obj) 
            => instance.startCompilingTime = Time.realtimeSinceStartup;
        private static void OnCompilationFinished(object obj)
        {
            var time = Time.realtimeSinceStartup - instance.startCompilingTime;
            if (instance.logCompilationDuration)
                Debug.Log($"Compiled for {time} seconds");
        }
        #endregion
    }
}
