using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace AutoDeleteFileCancel
{
    public static class BackgroundWorkerUtils
    {
        public static Task<object> ToTask
        (
            this BackgroundWorker backgroundWorker,
            CancellationTokenSource cancellationTokenSource = null,
            IProgress<object> progress = null
        )
        {
            TaskCompletionSource<object> taskCompletionSource =
                new TaskCompletionSource<object>(TaskCreationOptions.AttachedToParent);

            if (cancellationTokenSource != null)
            {
                // when the task is cancelled, 
                // trigger CancelAsync function on the background worker
                cancellationTokenSource.Token.Register
                (
                    () =>
                    {
                        if (backgroundWorker.WorkerSupportsCancellation)
                            backgroundWorker.CancelAsync();
                    }
                );
            }

            if (progress != null)
            {
                backgroundWorker.ProgressChanged += (sender, progressChangedArgs) =>
                {
                    progress.Report(progressChangedArgs.ProgressPercentage);
                };
            }

            RunWorkerCompletedEventHandler onCompleted = null;

            onCompleted = (object sender, RunWorkerCompletedEventArgs e) =>
            {
                backgroundWorker.RunWorkerCompleted -= onCompleted;

                if (e.Cancelled)
                {
                    // if the background worker was cancelled,
                    // set the Task as cancelled.  
                    taskCompletionSource.SetCanceled();
                    taskCompletionSource.SetException(new OperationCanceledException());
                }
                else if (e.Error != null)
                {
                    taskCompletionSource.SetException(e.Error);
                }
                else
                {
                    taskCompletionSource.SetResult(e.Result);
                }
            };

            backgroundWorker.RunWorkerCompleted += onCompleted;

            backgroundWorker.RunWorkerAsync();

            return taskCompletionSource.Task;
        }
    }
}
