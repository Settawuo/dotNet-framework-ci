using FBBPAYG_RevalueAssetBatch.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBBPAYG_RevalueAssetBatch
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try

            {
                Bootstrapper.Bootstrap();
                FBBPAYGRevalueAssetBatch jobs = Bootstrapper.GetInstance<FBBPAYGRevalueAssetBatch>();
                Console.WriteLine($"FBBPAYGRevalueAssetBatch Start");

                jobs.ProcessBatch();

                Console.WriteLine($"FBBPAYGRevalueAssetBatch End");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR :{ex.Message.ToString()}");
            }
        }
    }
}
