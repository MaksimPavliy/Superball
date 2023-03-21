#if DEBUG_PERFORMANCE
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace FriendsGamesTools.DebugTools
{
    public class PerformanceStatsView : MonoBehaviour
    {
        const float bInMb = 1024 * 1024;
        string ShowBInMB(long b) => $"{(b/bInMb).ToString("0.00")}mb";
        [SerializeField] TextOrTMPro text;
        public float updateInterval = 1;
        float nextUpdateTime;
        const string na = "N/A";
        string GetAllocatedMemoryForGraphicsDriver()
        {
            // Returns the amount of allocated memory for the graphics driver.
            if (!Debug.isDebugBuild)
                return na;
            return ShowBInMB(Profiler.GetAllocatedMemoryForGraphicsDriver());
        }
        string GetTotalReservedMemoryLong()
        {
            // The total memory Unity has reserved.
            var value = Profiler.GetTotalReservedMemoryLong();
            if (value == 0)
                return na;
            return ShowBInMB(value);
        }
        string GetTotalAllocatedMemoryLong()
        {
            //     The total memory allocated by the internal allocators in Unity. Unity reserves
            //     large pools of memory from the system. This function returns the amount of used
            //     memory in those pools.
            var value = Profiler.GetTotalAllocatedMemoryLong();
            if (value == 0)
                return na;
            return ShowBInMB(value);
        }
        string GetTotalUnusedReservedMemoryLong()
        {
            //     Unity allocates memory in pools for usage when unity needs to allocate memory.
            //     This function returns the amount of unused memory in these pools.
            var value = Profiler.GetTotalUnusedReservedMemoryLong();
            if (value == 0)
                return na;
            return ShowBInMB(value);
        }
        string GetMonoHeapSizeLong()
        {
            //     Returns the size of the reserved space for managed-memory.
            var value = Profiler.GetMonoHeapSizeLong();
            if (value == 0)
                return na;
            return ShowBInMB(value);
        }
        string GetUsedHeapSizeLong()
        {
            //     Returns the number of bytes that Unity has allocated. This does not include bytes
            //     allocated by external libraries or drivers.
            var value = Profiler.usedHeapSizeLong;
            if (value == 0)
                return na;
            return ShowBInMB(value);
        }
        string GetMonoUsedSizeLong()
        {
            //     The allocated managed-memory for live objects and non-collected objects.
            var value = Profiler.GetMonoUsedSizeLong();
            if (value == 0)
                return na;
            return ShowBInMB(value);
        }
        bool profilerEnabled => Profiler.enabled;
        private void Start()
        {
            Profiler.enabled = true;
        }
        void Update()
        {
            if (nextUpdateTime > Time.realtimeSinceStartup)
                return;
            nextUpdateTime = Time.realtimeSinceStartup + updateInterval;
            text.text =
                $"graphicsDeviceType={SystemInfo.graphicsDeviceType}\n" +
                $"Debug.isDebugBuild = {Debug.isDebugBuild}\n" +
                $"profiler {(profilerEnabled ? "enabled" : "disabled")}\n" +
                $"CPU UsagePC(does it work?) = \t{CPUPC.ReadCPUPC()}%\n" +
                $"CPU UsageAndroid(does it work?) = \t{CPUAndroid.ReadCPUAndroid()}%\n" +
                $"RAM memory = \t{SystemInfo.systemMemorySize}mb\n" + // Amount of system memory present.
                $"GPU memory = \t{SystemInfo.graphicsMemorySize}mb\n" + // Amount of video memory present.
                $"GPU allocated = \t{GetAllocatedMemoryForGraphicsDriver()}\n" +
                $"total reserved = \t{GetTotalReservedMemoryLong()}\n" +
                $"total used = \t{GetTotalAllocatedMemoryLong()}\n" +
                $"total unused = \t{GetTotalUnusedReservedMemoryLong()}\n" +
                $"code heap = \t{GetMonoHeapSizeLong()}\n" +
                $"code used 1 = \t{GetUsedHeapSizeLong()}\n" +
                $"code used 2 = \t{GetMonoUsedSizeLong()}\n" +
                "";
        }
    }
}
#endif