using LowUtilizeSaleReport.CompositionRoot;
using System;
using WBBEntity.Extensions;

namespace LowUtilizeSaleReport
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<LowUtilizeSaleReportJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                logger.Info("Start Execute LowUtilizeSaleReport");
                Console.WriteLine("Start Execute LowUtilizeSaleReport");
                job.Execute();
                logger.Info("End Execute LowUtilizeSaleReport");
                Console.WriteLine("End Execute LowUtilizeSaleReport");
            }
            catch (Exception ex)
            {
                var errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At LowUtilizeSaleReport");
                logger.Info(errorMessage);
                Console.WriteLine("Error At LowUtilizeSaleReport : " + errorMessage);
            }
        }
    }
}
