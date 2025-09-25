using FBBPAYG_LMD_SUBPAYMENT.CompositionRoot;
using System;

namespace FBBPAYG_LMD_SUBPAYMENT
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<Fbbpayglmdsubpayment>();
            try
            {
                Console.WriteLine("FBBPAYG_LMD_SUBPAYMENT start");
                job.log("Auto FBBPAYG_LMD_SUBPAYMENT start");

                job.ExecuteJob();
            }
            catch { }
            job.log("Auto FBBPAYG_LMD_SUBPAYMENT End");
            Console.WriteLine("FBBPAYG_LMD_SUBPAYMENT End");
        }

    }
}
