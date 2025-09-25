using FBSSOM010Notification.CompositionRoot;

namespace FBSSOM010Notification
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBSSOM010NotificationBatchJob>();
            job.ExecuteSendEmail();
        }
    }
}
