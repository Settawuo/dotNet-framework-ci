using System;
using WBBEntity.Extensions;
using FBBPAYG_LoadFile_3BBReport.CompositionRoot;

namespace FBBPAYG_LoadFile_3BBReport
{

    internal class Program
    {
        private static string _errorMessage = "";

        public static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            FBBPayG_LoadFile_3BBReportBatch instance = Bootstrapper.GetInstance<FBBPayG_LoadFile_3BBReportBatch>();
            var temp = Bootstrapper.GetInstance<FBBPayG_LoadFile_3BBReportBatch>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();
            try
            {
                temp.ExecuteJob();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At FBBPAYG_LoadFile_3BBReport");
                logger.Info(_errorMessage);
            }
            //Console.ReadLine();


        }
    }
}
