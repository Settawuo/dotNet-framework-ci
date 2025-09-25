using FBBPAYGExpireSIM.CompositionRoot;
using System;

namespace FBBPAYGExpireSIM
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Bootstrapper.Bootstrap();
                FBBPAYGExpireSIMBatch jobs = Bootstrapper.GetInstance<FBBPAYGExpireSIMBatch>();
                Console.WriteLine($"FBBPAYGExpireSIM Start");
                var result_process = jobs.CheckProcessBatch();
                if (result_process)
                {
                    jobs.GenFiles();
                }
                Console.WriteLine($"FBBPAYGExpireSIM End");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR :{ex.Message.ToString()}");
            }
        }
    }
}
