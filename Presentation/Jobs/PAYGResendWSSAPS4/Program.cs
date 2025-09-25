using PAYGResendWSSAPS4.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAYGResendWSSAPS4
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<PAYGResendWSSAPS4Job>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();
            try
            {
                    
                logger.Info("Start At PAYGResendWSSAPS4");
              var program_process = job.Get_FBSS_CONFIG_TBL_LOV("PAYG_RESENDWSSAPS4_BATCH", "PROGRAM_PROCESS").FirstOrDefault();

                logger.Info("Program Process: "+ program_process.DISPLAY_VAL);
                if (program_process.DISPLAY_VAL == "Y")
                {
                    job.ResendPendingPAYGResendWSSAPS4();
                }

                logger.Info("PAYGResendWSSAPS4 End");
            }
            catch (Exception ex)
            {
                logger.Info("Exception At Program PAYGResendWSSAPS4Job " + ex.Message);
            }
        }
    }
}
