using FBBSubmitFOALogSendMailSAPS4.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBBSubmitFOALogSendMailSAPS4
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBSubmitFOALogSendMailSAPS4Job>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();
            try
            {

                logger.Info("Start At FBBSubmitFOALogSendMailSAPS4");
                var program_process = job.Get_FBSS_CONFIG_TBL_LOV("FBB_SUBMITFOALOGSENDMAILSAPS4_BATCH", "PROGRAM_PROCESS").FirstOrDefault();

                logger.Info("Program Process: " + program_process.DISPLAY_VAL);
                if (program_process.DISPLAY_VAL == "Y")
                {
                    job.FBBSubmitFOALogSendMailSAPS4();
                }

                logger.Info("FBBSubmitFOALogSendMailSAPS4 End");
            }
            catch (Exception ex)
            {
                logger.Info("Exception At Program FBBSubmitFOALogSendMailSAPS4 " + ex.Message);
            }
        }
    }
}
