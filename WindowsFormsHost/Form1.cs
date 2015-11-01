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
            string result = preTask.BlockingCall();

            MessageBox.Show(result);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            string result = preTask.AsyncCallWithWait();

            var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            MessageBox.Show(result);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            preTask.ASyncCallWithCallback(result =>
            {
                var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

                MessageBox.Show(result);
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            string result = dotNet4Task.AsyncCallWithWait();

            var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            MessageBox.Show(result);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            dotNet4Task.ASyncCallWithContinuation().ContinueWith(task =>
            {
                var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

                MessageBox.Show(task.Result);
            }); // fix: }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            string result = await dotNet45Task.ASyncCallWithContinuation();

            var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            MessageBox.Show(result);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // let's assume that we can't change method signature to async so we need to use blocking

            var entryThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            string result = dotNet45Task.ASyncCallWithContinuation().Result; // fix: see DotNet45TaskExamples.ASyncCallWithContinuation() source

            var returningThreadInfo = ThreadInfo.GetCurrentThreadInfo();

            MessageBox.Show(result);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string result = dotNet45Task.HiddenAsyncCallWithBlock(); // fix: see DotNet45TaskExamples.HiddenAsyncCallWithBlock() source

            MessageBox.Show(result);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            dotNet45Task.ASyncCallWithException().Wait();
            // fix: change .Wait() to .GetAwaiter().GetResult() if we have to block or await dotNet45Task.ASyncCallWithException() if we can use async

            // here try also: dotNet45Task.ASyncVoidCallWithException().Wait(), for fix see DotNet45TaskExamples.ASyncVoidCallWithException() source
        }

        private void button10_Click(object sender, EventArgs e)
        {
            preTask.BackgroundProcessing();
        }

        private void button11_Click(object sender, EventArgs e)
        {
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
            string result = await dotNet45Task.BackgroundProcessingWithTimeout(2000);

            MessageBox.Show(result);
        }
    }
}