using System.Threading.Tasks;
using CurrentExamples;
using Utilities;

namespace ConsoleHost
{
    class Program
    {
        static readonly DotNet45TaskExamples dotNet45Task = new DotNet45TaskExamples();

        static void Main(string[] args)
        {
            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            string result = dotNet45Task.ASyncCallWithContinuation().GetAwaiter().GetResult();

            var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();
        }

        static async Task MainAsync(string[] args)
        {
            // some logic here

            string result = await dotNet45Task.ASyncCallWithContinuation().ConfigureAwait(false);

            // some logic here
        }
    }
}