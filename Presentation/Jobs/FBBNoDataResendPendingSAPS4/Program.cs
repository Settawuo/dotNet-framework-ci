using FBBNoDataResendPendingSAPS4;
using FBBNoDataResendPendingSAPS4.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBBNoDataResendPendingSAPS4
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBNoDataResendPendingSAPS4Job>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();
            try
            {

                logger.Info("Start At FBBNoDataResendPendingSAPS4");
                var program_process = job.Get_FBSS_CONFIG_TBL_LOV("FBBNoDataResendPendingSAPS4_Batch", "PROGRAM_PROCESS").FirstOrDefault();

                logger.Info("Program Process: " + program_process.DISPLAY_VAL);
                if (program_process.DISPLAY_VAL == "Y")
                {
                    job.ResendPendingS4();
                }
                logger.Info("FBBNoDataResendPendingSAPS4 End");
            }
            catch (Exception ex)
            {
                logger.Info("Exception At Program FBBNoDataResendPendingSAPS4 " + ex.Message);
            }
        }
    }
}
