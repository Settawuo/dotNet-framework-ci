using FBBReconcileInvRPT.CompositionRoot;
using System;

namespace FBBReconcileInvRPT
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBReconcileInvRPTBatchJob>();


            Console.WriteLine("FBBReconcileInvRPT start");
            job.SendMail();
            Console.WriteLine("FBBReconcileInvRPT end");
        }
    }
}
