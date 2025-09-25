using FBBPAYGLoadSIMS4Batch.CompositionRoot;
using System;
using WBBEntity.Extensions;

namespace FBBPAYGLoadSIMS4Batch
{
    internal class Program
    {
        private static string _errorMessage = "";

        private static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            Console.WriteLine($"FBBPAYGLoadSIMS4Batch Start");

            var logger = Bootstrapper.GetInstance<DebugLogger>();
            FBBPAYGLoadSIMS4BatchMain instance = Bootstrapper.GetInstance<FBBPAYGLoadSIMS4BatchMain>();

            try
            {
                instance.ExecuteJob();
                Console.WriteLine($"FBBPAYGLoadSIMS4Batch End");
            }
            catch (Exception ex)
            {
                _errorMessage = ex.RenderExceptionMessage();
                logger.Error("Error At FBBPAYGLoadSIMS4Batch");
                logger.Error(_errorMessage);
            }
        }
    }
}
