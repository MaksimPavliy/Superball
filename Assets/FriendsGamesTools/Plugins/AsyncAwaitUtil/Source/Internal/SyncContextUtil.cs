using System.Threading;

namespace UnityAsyncAwaitUtil
{
    public static class SyncContextUtil
    {
        static bool inited;
        static void InitIfNeeded()
        {
            if (inited) return;
            _UnitySynchronizationContext = SynchronizationContext.Current;
            _UnityThreadId = Thread.CurrentThread.ManagedThreadId;
            inited = true;
        }

        static int _UnityThreadId;
        public static int UnityThreadId { get {
                InitIfNeeded();
                return _UnityThreadId;
            }
        }

        static SynchronizationContext _UnitySynchronizationContext;
        public static SynchronizationContext UnitySynchronizationContext { get {
                InitIfNeeded();
                return _UnitySynchronizationContext;
            }
        }
    }
}

