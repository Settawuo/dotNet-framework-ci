using FBBPAYG_ReconcileFBSS_CPE.CompositionRoot;
using System;
using System.Linq;
using WBBEntity.Extensions;

namespace FBBPAYG_ReconcileFBSS_CPE
{
    class Program
    {
        private static string _errorMessage = "";

        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBBPAYG_ReconcileFBSS_CPE>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                logger.Info("Start At FBBPAYG_ReconcileFBSS_CPE");
                var PROGRAM_PROCESS = job.Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_RECONCILEFBSS_CPE", "PROGRAM_PROCESS").FirstOrDefault();

                if (PROGRAM_PROCESS != null)
                {
                    logger.Info("FBBPAYG_ReconcileFBSS_CPE PROGRAM_PROCESS : " + PROGRAM_PROCESS.DISPLAY_VAL);
                    if (PROGRAM_PROCESS.DISPLAY_VAL == "Y")
                    {
                        job.ExecuteJob();
                    }
                }
                else
                {
                    logger.Info("FBBPAYG_ReconcileFBSS_CPE PROGRAM_PROCESS : Null");
                }

            }
            catch (Exception ex)
            {
                _errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At FBBPAYG_ReconcileFBSS_CPE");
                logger.Info(_errorMessage);
            }

        }
    }
}
