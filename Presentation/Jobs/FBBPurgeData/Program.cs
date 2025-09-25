using FBBPurgeData.CompositionRoot;
using System;

namespace FBBPurgeData
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try

            {
                Bootstrapper.Bootstrap();
                FBBPurgeDataBatch jobs = Bootstrapper.GetInstance<FBBPurgeDataBatch>();
                Console.WriteLine($"FBBPurgeData Start");

                jobs.CheckProcessBatch();

                Console.WriteLine($"FBBPurgeData End");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR :{ex.Message.ToString()}");
            }
        }
    }
}
