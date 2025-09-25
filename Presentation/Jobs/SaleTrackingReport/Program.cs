using SaleTrackingReport.CompositionRoot;
using System;
using WBBEntity.Extensions;

namespace SaleTrackingReport
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<SaleTrackingReportJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                job.Execute();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At FBBAutoMailReport");
                logger.Info(errorMessage);
            }
        }
    }
}
