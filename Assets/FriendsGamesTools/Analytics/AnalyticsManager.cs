#if ANALYTICS
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.Analytics
{
    public class AnalyticsManager : MonoBehaviourHasInstance<AnalyticsManager>
    {
        static AnalyticsSettings settings => AnalyticsSettings.instance;
        public static List<Type> GetSourceTypes(bool editModeWay)
        {
            Debug.Assert(editModeWay != Application.isPlaying);
            var types = ReflectionUtils.GetAllDerivedTypes(typeof(AnalyticsSource));
            types = types.Filter(t =>
            {
                if (!t.CanCreateInstance())
                    return false;
                AnalyticsSource instance;
                if (!editModeWay)
                    instance = AnalyticsSource.sources.Find(s => s.GetType() == t);
                else
                    instance = ReflectionUtils.CreateInstance<AnalyticsSource>(t);
                var enabled = !settings.disabledAnalytics.Contains(instance.ModuleName);
                if (editModeWay)
                    instance.Destroy();
                return enabled;
            });
            return types;
        }
        static List<AnalyticsSource> usedSources;
        private void Start()
        {
            // analytics managers set their instances in awake, here they are collected in start.
            usedSources = GetUsedSources();
        }
        public List<AnalyticsSource> GetUsedSources()
        {
            var usedSources = new List<AnalyticsSource>();
            var types = GetSourceTypes(false);
            types.ForEach(t =>
            {
                var source = AnalyticsSource.sources.Find(s => s.GetType() == t);
                Debug.Assert(source != null, $"analytics source with type {t.Name} not found");
                usedSources.Add(source);
            });
            return usedSources;
        }
        static bool ready => usedSources != null && instance != null;
        public static void Send(string name, params (string key, object value)[] parameters)
        {
            if (ready)
                SendImmediately(name, parameters);
            else
                SendWhenReady(name, parameters);
        }
        private static void SendImmediately(string name, params (string key, object value)[] parameters)
        {
            usedSources.ForEach(s=> {
                if (s.ready)
                    s.Send(name, parameters);
                else
                    SendWhenReady(s, name, parameters);
            });
        }
        private async static void SendWhenReady(string name, params (string key, object value)[] parameters)
        {
            while (!ready)
                await Awaiters.EndOfFrame;
            SendImmediately(name, parameters);
        }
        private async static void SendWhenReady(AnalyticsSource source, string name, params (string key, object value)[] parameters)
        {
            while (!source.ready)
                await Awaiters.EndOfFrame;
            source.Send(name, parameters);
        }
    }
    public abstract class AnalyticsSource
    {
        static List<AnalyticsSource> _sources = new List<AnalyticsSource>();
        public static IEnumerable<AnalyticsSource> sources => _sources;
        public AnalyticsSource() => _sources.Add(this);
        public void Destroy() => _sources.Remove(this);
        public abstract string ModuleName { get; }
        public virtual bool ready => true;
        public abstract void Send(string eventName, params (string key, object value)[] parameters);
    }
}
#endif