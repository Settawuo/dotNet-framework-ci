using FBBInventoryResendPendingSAPS4.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBBInventoryResendPendingSAPS4
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBInventoryResendPendingSAPS4Job>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();
            try
            {
                    
                logger.Info("Start At FBBInventoryResendPendingSAPS4");
                var program_process = job.Get_FBSS_CONFIG_TBL_LOV("FBBInventoryResendPendingSAPS4_Batch", "PROGRAM_PROCESS").FirstOrDefault();

                logger.Info("Program Process: "+ program_process.DISPLAY_VAL);
                if (program_process.DISPLAY_VAL == "Y")
                {
                    job.ResendPendingS4();
                }

                logger.Info("FBBInventoryResendPendingSAPS4 End");
            }
            catch (Exception ex)
            {
                logger.Info("Exception At Program FBBInventoryResendPendingSAPS4Job "+ex.Message);
            }
        }
    }
}
