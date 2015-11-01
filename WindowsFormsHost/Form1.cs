using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CurrentExamples;
using LegacyExamples;
using Utilities;

namespace WindowsFormsHost
{
    public partial class Form1 : Form
    {
        readonly DotNet45TaskExamples dotNet45Task = new DotNet45TaskExamples();
        readonly DotNet4TaskExamples dotNet4Task = new DotNet4TaskExamples();
        readonly PreTaskExamples preTask = new PreTaskExamples();

        public Form1()
        {
            InitializeComponent();

            var monitor = new ThreadPoolMonitor();
            monitor.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // shows how asynchronous I/O should NOT be implemented :)

            string result = preTask.BlockingCall();

            MessageBox.Show(result);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // shows how asynchronous I/O executes using I/O completion ports (watch debug output), but still blocks executing thread

            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            string result = preTask.AsyncCallWithWait();

            var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            MessageBox.Show(result);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // shows how asynchronous programming was in APM (callback hell and synchronization problems)

            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            preTask.ASyncCallWithCallback(result =>
            {
                var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

                MessageBox.Show(result);
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // shows blocking TPM approach to asynchronous calls (no gain over APM)

            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            string result = dotNet4Task.AsyncCallWithWait();

            var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            MessageBox.Show(result);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // shows non blocking TPM approach to asynchronous calls (but still with callbacks and synchronization problems)

            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            dotNet4Task.ASyncCallWithContinuation().ContinueWith(task =>
            {
                var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

                MessageBox.Show(task.Result);
            }); // fix: }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            // shows how using async/await can simplify asynchronous calls (and context synchronization)

            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            string result = await dotNet45Task.ASyncCallWithContinuation();

            var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            MessageBox.Show(result);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // shows how default context synchronization used in conjunction with blocking can cause a deadlock

            // let's assume that we can't change method signature to async so we need to use blocking

            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            string result = dotNet45Task.ASyncCallWithContinuation().Result; // fix: see DotNet45TaskExamples.ASyncCallWithContinuation() source

            var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            MessageBox.Show(result);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // shows why asynchronous call blocking (to provide synchronous API) should be avoided (again - deadlock)

            string result = dotNet45Task.HiddenAsyncCallWithBlock(); // fix: see DotNet45TaskExamples.HiddenAsyncCallWithBlock() source

            MessageBox.Show(result);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // shows how .NET 4.5 Task API is better at exception handling than .NET 4.0 API (and also why async void is evil)

            dotNet45Task.ASyncCallWithException().Wait();
            // fix: change .Wait() to .GetAwaiter().GetResult() if we have to block or await dotNet45Task.ASyncCallWithException() if we can use async

            // try calling: dotNet45Task.ASyncVoidCallWithException().Wait(), for fix see DotNet45TaskExamples.ASyncVoidCallWithException() source
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // shows oldschool approach to ThreadPool processing ;)

            preTask.BackgroundProcessing();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            // shows why Task.Factory.StartNew default settings are dangerous

            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            dotNet4Task.BackgroundProcessing().ContinueWith(task =>
            {
                var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

                // second call - watch thread info
                dotNet4Task.BackgroundProcessing(); // fix: see DotNet4TaskExamples.BackgroundProcessing() source

                MessageBox.Show(task.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async void button12_Click(object sender, EventArgs e)
        {
            // shows how to perform ThreadPool calculations in .NET 4.5

            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            var processingCancellation = new CancellationTokenSource();

            //processingCancellation.Cancel(); // causes TaskCancelledException

            Task<string> processingTask = dotNet45Task.BackgroundProcessing(processingCancellation.Token);

            //Thread.Sleep(2000);
            //processingCancellation.Cancel(); // causes OperationCancelledException

            string result = await processingTask;

            var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            MessageBox.Show(result);
        }

        private async void button13_Click(object sender, EventArgs e)
        {
            // shows how to implement timeout handling in asynchronous operations

            string result = await dotNet45Task.BackgroundProcessingWithTimeout(2000);

            MessageBox.Show(result);
        }
    }
}