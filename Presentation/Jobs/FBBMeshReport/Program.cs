using FBBMeshReport.CompositionRoot;
using System;
using WBBEntity.Extensions;

namespace FBBMeshReport
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBBMeshReportJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                logger.Info("Start Execute FBBMeshReport");
                Console.WriteLine("Start Execute FBBMeshReport");

                job.Execute();

                logger.Info("End Execute FBBMeshReport");
                Console.WriteLine("End Execute FBBMeshReport");
            }
            catch (Exception ex)
            {
                var errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At FBBMeshReport");
                logger.Info(errorMessage);
                Console.WriteLine("Error At FBBMeshReport : " + errorMessage);
            }
        }
    }
}
