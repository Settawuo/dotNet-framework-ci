using System;
using WebServiceLostTransaction.CompositionRoot;

namespace WebServiceLostTransaction
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WebServiceLostTransaction start");
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<WebserviceLostTransactionJob>();
            job.JobExcute();
        }
    }
}
