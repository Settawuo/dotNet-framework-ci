using System;
using WBBEntity.Extensions;

namespace StandardAddressReport
{
    using StandardAddressReport.CompositionRoot;

    class Program
    {
        private static string _errorMessage = "";
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var temp = Bootstrapper.GetInstance<StandardAddressReportJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                logger.Info("Start At StandardAddressReport");
                temp.StandardAddressReport();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At StandardAddressReport");
                logger.Info(_errorMessage);
                logger.Info(ex.Message);
            }
        }
    }
}
