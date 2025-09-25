using FBBMonitorLmr.CompositionRoot;
using System;

namespace FBBMonitorLmr
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBMonitorLmrBatchJob>();


            Console.WriteLine("Auto FBBMonitorLmr start");
            job.ResendPending();
            job.ResendPendingInstall();
            Console.WriteLine("Auto FBBMonitorLmr end");
        }
    }
}
