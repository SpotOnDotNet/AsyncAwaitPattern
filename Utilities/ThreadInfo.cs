using System.Threading;
using System.Web;

namespace Utilities
{
    public class ThreadInfo
    {
        ThreadInfo()
        {
        }

        public bool IsBackground { get; private set; }
        public bool IsFromPool { get; private set; }
        public bool IsHttpContextPresent { get; private set; }
        public bool IsUIThread { get; private set; }
        public int ThreadID { get; private set; }

        public static ThreadInfo GetCurrentThreadInfo()
        {
            Thread currentThread = Thread.CurrentThread;

            return new ThreadInfo
            {
                ThreadID = currentThread.ManagedThreadId,
                IsFromPool = currentThread.IsThreadPoolThread,
                IsBackground = currentThread.IsBackground,
                IsUIThread = !currentThread.IsBackground && !currentThread.IsThreadPoolThread && SynchronizationContext.Current != null,
                IsHttpContextPresent = HttpContext.Current != null
            };
        }
    }
}