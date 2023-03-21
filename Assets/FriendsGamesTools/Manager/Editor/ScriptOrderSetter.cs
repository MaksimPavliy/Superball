using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools
{
    public class ScriptOrderSetter
    {
        protected virtual List<(Type type, int order)> order => new List<(Type type, int order)> {
#if ECS_TRAJECTORIES
            (typeof(ECSGame.TrajectoriesView), -20), // Trajectories should init before ecs controllers.
#endif
#if DEBUG_CONFIG
            (typeof(DebugTools.BalanceSettings<>), -10), // Balance should awake before any actual game does.
#endif
#if ECS_GAMEROOT
            (typeof(ECSGame.GameRoot), 250), // This automatically makes GameRoot scripts execute after default order.
#endif
        };
        public static void InitOnLoad()
        {
            var instTypes = ReflectionUtils.GetAllDerivedTypes<ScriptOrderSetter>();
            instTypes.Remove(typeof(ScriptOrderSetter));
            if (instTypes.Count > 1)
            {
                Debug.LogError($"there should be no more than 1 script " +
                    $"derived from ScriptOrderSetter, but found {instTypes.Count}:" +
                    $"{instTypes.ConvertAll(t => t.FullName).PrintCollection(", ")}");
                return;
            }
            var instType = instTypes.Count > 0 ? instTypes[0] : typeof(ScriptOrderSetter);
            var inst = ReflectionUtils.CreateInstance<ScriptOrderSetter>(instType);
            var items = inst.order;
            foreach (var (type, order) in items)
            {
                var types = ReflectionUtils.GetAllDerivedTypes(type);
                types.ForEach(SetOrderIfNeeded);
                SetOrderIfNeeded(type);
                void SetOrderIfNeeded(Type currType)
                {
                    if (!currType.CanCreateInstance())
                        return;
                    var monoScript = new ExampleScript(currType.Name).asset;
                    SetOrder(monoScript, order);
                }
            }
            SetFirstAndLastScripts();
        }
        static void SetOrder(MonoScript monoScript, int order)
        {
            int currentExecutionOrder = MonoImporter.GetExecutionOrder(monoScript);
            if (currentExecutionOrder != order)
                MonoImporter.SetExecutionOrder(monoScript, order);
        }
        static void SetFirstAndLastScripts()
        {
#if DEBUG_PERFORMANCE
            var allScripts = MonoImporter.GetAllRuntimeMonoScripts();
            var orders = allScripts.ConvertAll(s=> MonoImporter.GetExecutionOrder(s));
            int min = int.MaxValue;
            int max = int.MinValue;
            var minType = typeof(DebugTools.CPUTimeMeasuringFirstMonobehaviour);
            var maxType = typeof(DebugTools.CPUTimeMeasuringLastMonobehaviour);
            var minTypes = allScripts.Filter(s=>s.name.Contains("CPUTimeMeasuringFirstMonobehaviour"));
            MonoScript minAsset = null, maxAsset = null;
            for (int i=0;i<allScripts.Length;i++)
            {
                if (allScripts[i].name == minType.Name)
                    minAsset = allScripts[i];
                else if (orders[i] < min)
                    min = orders[i];

                if (allScripts[i].name == maxType.Name)
                    maxAsset = allScripts[i];
                else if (orders[i] > max)
                    max = orders[i];
            }
            min--;
            max++;
            SetOrder(minAsset, min);
            SetOrder(maxAsset, max);
#endif
        }
    }
}