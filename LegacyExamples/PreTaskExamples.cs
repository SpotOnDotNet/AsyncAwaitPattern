using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Utilities;
using static Utilities.HttpWebResponseExtensions;

namespace LegacyExamples
{
    public class PreTaskExamples
    {
        public void ASyncCallWithCallback(Action<string> handler)
        {
            HttpWebRequest request = CreateRequest();

            request.BeginGetResponse(asyncResult =>
            {
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);

                string result = response.GetContent();

                handler(result);
            }, null);
        }

        public string AsyncCallWithWait()
        {
            HttpWebRequest request = CreateRequest();
            IAsyncResult asyncResult = request.BeginGetResponse(null, null);

            asyncResult.AsyncWaitHandle.WaitOne();

            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);
            return response.GetContent();
        }

        public string BlockingCall()
        {
            HttpWebRequest request = CreateRequest();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return response.GetContent();
        }

        public void BackgroundProcessing()
        {
            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            var worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            worker.DoWork += (sender, e) =>
            {
                var threadInfo = ThreadInfo.GetCurrentThreadInfo();

                for (int i = 1; i <= 100; i++)
                {
                    Thread.Sleep(50);
                    worker.ReportProgress(i);
                }
            };

            worker.ProgressChanged += (sender, e) =>
            {
                Debug.WriteLine(e.ProgressPercentage);
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                var threadInfo = ThreadInfo.GetCurrentThreadInfo();
            };

            worker.RunWorkerAsync();
        }

        HttpWebRequest CreateRequest()
        {
            return (HttpWebRequest)WebRequest.Create("http://localhost:8088/slow/?seconds=5");
        }
    }
}