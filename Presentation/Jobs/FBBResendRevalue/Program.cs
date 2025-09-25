using FBBResendRevalue.CompositionRoot;
using System;

namespace FBBResendRevalue
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBResendRevalueBatchJob>();


            Console.WriteLine("FBBResendRevalue start");
            job.ResendRevalue();
            Console.WriteLine("FBBResendRevalue end");
        }
    }
}
