using FBBReturnedRevalueAssetS4HANA.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBBReturnedRevalueAssetS4HANA
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try

            {
                Bootstrapper.Bootstrap();
                FBBReturnedRevalueAssetS4HANAJob jobs = Bootstrapper.GetInstance<FBBReturnedRevalueAssetS4HANAJob>();
                Console.WriteLine($"FBBReturnedRevalueAssetS4HANA Start");

                jobs.ProcessBatch();

                Console.WriteLine($"FBBReturnedRevalueAssetS4HANA End");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR :{ex.Message.ToString()}");
            }
        }
    }
}
