using GenReportReconcileBatch.CompositionRoot;
using System;
using WBBEntity.Extensions;

namespace GenReportReconcileBatch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<GenReportReconcileBatchJob>();
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
