using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace CurrentExamples
{
    public class DotNet45TaskExamples
    {
        readonly HttpClient httpClient = new HttpClient();

        public async Task<string> ASyncCallWithContinuation()
        {
            // some logic here

            string result = await CallService(); // fix: await CallService().ContinueAwait(false);

            // some logic here

            return result;
        }

        public string HiddenAsyncCallWithBlock() // fix: public async Task<string> HiddenAsyncCallWithBlock()
        {
            Task<string> task1 = ASyncCallWithContinuation();
            Task<string> task2 = ASyncCallWithContinuation();

            Task.WaitAll(task1, task2); // fix: await Task.WhenAll(task1, task2).ConfigureAwait(false);

            return task1.Result + task2.Result; // fix: return await task1.ConfigureAwait(false) + await task2.ConfigureAwait(false);
        }

        public async Task ASyncCallWithException()
        {
            // some logic here

            await CallServiceWithException().ConfigureAwait(false);

            // some logic here
        }

        public async void ASyncVoidCallWithException() // fix: public async Task ASyncVoidCallWithException()
        {
            // some logic here

            await CallServiceWithException().ConfigureAwait(false);

            // some logic here
        }

        public Task<string> BackgroundProcessing(CancellationToken cancellationToken)
        {
            var progressHandler = new Progress<int>(value =>
            {
                Debug.WriteLine(value);
            });

            var progress = (IProgress<int>)progressHandler;

            return Task.Run(() =>
            {
                var threadInfo = ThreadInfo.GetCurrentThreadInfo();

                for (int i = 1; i <= 100; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Thread.Sleep(50);
                    progress.Report(i);
                }

                return "ok";
            }, cancellationToken);
        }

        public async Task<string> BackgroundProcessingWithTimeout(int miliseconds)
        {
            string result = null;

            CancellationTokenSource backgroundTaskCancellation = new CancellationTokenSource();

            Task<string> backgroundTask = BackgroundProcessing(backgroundTaskCancellation.Token);

            if (await Task.WhenAny(backgroundTask, Task.Delay(miliseconds)).ConfigureAwait(false) != backgroundTask)
            {
                backgroundTaskCancellation.Cancel();
            }

            try
            {
                result = await backgroundTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException();
            }

            return result;
        }

        Task<string> CallService(int delay = 5)
        {
            return httpClient.GetStringAsync($"http://localhost:8088/slow/?seconds={delay}");
        }

        Task CallServiceWithException()
        {
            throw new InvalidOperationException("some exception");
        }
    }
}