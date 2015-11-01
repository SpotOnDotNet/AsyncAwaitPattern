using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace LegacyExamples
{
    public class DotNet4TaskExamples
    {
        public Task<string> ASyncCallWithContinuation()
        {
            Task<WebResponse> serviceCallTask = CallService();

            var resultSource = new TaskCompletionSource<string>();

            serviceCallTask.ContinueWith(previousTask =>
            {
                HttpWebResponse response = (HttpWebResponse)previousTask.Result;

                string result = response.GetContent();

                resultSource.SetResult(result);
            });

            return resultSource.Task;
        }

        public string AsyncCallWithWait()
        {
            Task<WebResponse> serviceCallTask = CallService();

            // serviceCallTask.Wait(); // no need to use .Wait() because .Result blocks by itself

            HttpWebResponse response = (HttpWebResponse)serviceCallTask.Result;

            return response.GetContent();
        }

        public Task<string> BackgroundProcessing()
        {
            return Task.Factory.StartNew(() =>
            {
                var threadInfo = ThreadInfo.GetCurrentThreadInfo();

                for (int i = 1; i <= 100; i++)
                {
                    Thread.Sleep(50);
                }

                return "ok";
            }); // fix: }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default
        }

        public Task<string> BackgroundProcessingAlternative()
        {
            var backgroundTask = new Task<string>(() =>
            {
                var threadInfo = ThreadInfo.GetCurrentThreadInfo();

                for (int i = 1; i <= 100; i++)
                {
                    Thread.Sleep(50);
                }

                return "ok";
            });

            backgroundTask.Start(TaskScheduler.Default);

            return backgroundTask;
        }

        Task<WebResponse> CallService()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8088/slow/?seconds=5");

            return Task.Factory.FromAsync(request.BeginGetResponse(null, null), request.EndGetResponse);
        }
    }
}