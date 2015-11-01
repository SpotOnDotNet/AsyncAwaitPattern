using System.Diagnostics;
using System.Threading;

namespace Utilities
{
    public class ThreadPoolMonitor
    {
        public void Start()
        {
            var monitoringThread = new Thread(() => Monitor());
            monitoringThread.Start();
        }

        void Monitor()
        {
            var stopwatch = new Stopwatch();

            int lastPoolWorkersLeft = 0;
            int lastPoolPortsLeft = 0;
            int poolWorkersLeft,
                poolPortsLeft,
                poolWorkersMax,
                poolPortsMax;

            ThreadPool.GetMaxThreads(out poolWorkersMax, out poolPortsMax);

            stopwatch.Start();

            while (true)
            {
                ThreadPool.GetAvailableThreads(out poolWorkersLeft, out poolPortsLeft);

                if (poolWorkersLeft != lastPoolWorkersLeft || poolPortsLeft != lastPoolPortsLeft)
                {
                    Debug.WriteLine($"time: {stopwatch.ElapsedMilliseconds,6}, threads: {poolWorkersMax - poolWorkersLeft,2}, compl. port threads: {poolPortsMax - poolPortsLeft,2}");

                    lastPoolWorkersLeft = poolWorkersLeft;
                    lastPoolPortsLeft = poolPortsLeft;
                }
            }
        }
    }
}