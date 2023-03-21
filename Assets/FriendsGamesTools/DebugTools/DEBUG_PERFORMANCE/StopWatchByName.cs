#if DEBUG_PERFORMANCE
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FriendsGamesTools.DebugTools
{
    public static class StopWatchByName
    {
        static Stopwatch sw;
        static Dictionary<string, (long ticks, int count)> finished;
        static Dictionary<string, long> started;
        static void InitIfNeeded()
        {
            if (sw != null)
                return;
            sw = new Stopwatch();
            sw.Start();
            finished = new Dictionary<string, (long ticks, int count)>();
            started = new Dictionary<string, long>();
        }
        public static bool IsStarted(string name) => started.ContainsKey(name);
        public static void Start(string name)
        {
            InitIfNeeded();
            Debug.Assert(!IsStarted(name), $"{name} should not be started");
            started.Add(name, sw.ElapsedTicks);
        }
        public static void Stop(string name)
        {
            Debug.Assert(IsStarted(name), $"{name} should be started");
            var elapsed = sw.ElapsedTicks - started[name];
            started.Remove(name);
            finished.TryGetValue(name, out var item);
            item.count++;
            item.ticks += elapsed;
            finished[name] = item;
        }
        public static void StopStart(string nameToStop, string nameToStart) {
            Stop(nameToStop);
            Start(nameToStart);
        }
        private static long GetMS(long ticks) => ticks / TimeSpan.TicksPerMillisecond;
        public static List<(string name, long ms, int count)> GetData()
        {
            var res = new List<(string name, long ms, int count)>();
            foreach (var (name, (ticks, count)) in finished)
                res.Add((name, GetMS(ticks), count));
            res.SortBy(item => item.ms, true);
            return res;
        }
        public static long GetMs(string name) => finished.TryGetValue(name, out var val) ? GetMS(val.ticks) : -1;
        public static void Log()
            => UnityEngine.Debug.Log(GetData().ConvertAll(d => $"{d.name} {d.ms * 0.001f}s x{d.count}").PrintCollection("\n"));
        public static void Clear()
        {
            InitIfNeeded();
            finished.Clear();
            started.Clear();
        }
    }
}
#endif