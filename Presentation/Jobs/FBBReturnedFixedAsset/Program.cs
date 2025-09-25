using FBBReturnedFixedAsset.CompositionRoot;
using System;

namespace FBBReturnedFixedAsset
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBReturnedFixedAssetJob>();
            job.ReturnedFixedAsset();
            Console.WriteLine("Executed FBBReturnedFixedAssetJob batch");
        }
    }
}
