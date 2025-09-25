using FBBNoDataResendPending.CompositionRoot;
using System;
using System.Linq;
using WBBEntity.Extensions;

namespace FBBNoDataResendPending
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Bootstrapper.Bootstrap();
                var job = Bootstrapper.GetInstance<FBBNoDataResendPendingBatchJob>();
                var logger = Bootstrapper.GetInstance<DebugLogger>();
                Console.WriteLine("Auto FBBNoDataResendPending start");

                var PROCESS_ROLLBACK = job.Get_FBB_CFG_LOV("FBB_NODATARESENDPENDING_BATCH", "ROLLBACK");

                //ถ้าเป็น Y ใช้ method ใหม่ R21.03.2021
                if (PROCESS_ROLLBACK == "Y")
                {
                    var date_div = job.Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "DATE_DIV").FirstOrDefault();
                    int Check_DateDiv = date_div.DISPLAY_VAL.ToSafeString() == "Y" ? date_div.VAL1.ToSafeInteger() + 1 : 1;
                    if (Check_DateDiv <= 0)
                    {
                        Check_DateDiv = 1;
                    }
                    string date_time_start_process = DateTime.Now.AddDays(-Check_DateDiv).ToString("ddMMyyyy HHmmss");
                    logger.Info("date_time_start_process " + date_time_start_process);
                    job.ResendPendingNewR2106();
                    job.ResendPendingInstallNewR2106(date_time_start_process);
                }
                else
                {
                    // R2111
                    var date_div = job.Get_FBSS_CONFIG_TBL_LOV("FBB_NODATARESENDPENDING_BATCH", "DATE_DIV").FirstOrDefault();
                    int Check_DateDiv = date_div.DISPLAY_VAL.ToSafeString() == "Y" ? date_div.VAL1.ToSafeInteger() + 1 : 1;
                    Check_DateDiv = Check_DateDiv <= 0 ? 1 : Check_DateDiv;

                    string date_time_start_process = DateTime.Now.AddDays(-Check_DateDiv).ToString("ddMMyyyy HHmmss");
                    logger.Info("date_time_start_process " + date_time_start_process);
                    job.ResendPendingNewR2111();
                    job.ResendPendingInstallNewR2111(date_time_start_process);
                }


                Console.WriteLine("Auto FBBNoDataResendPending end");
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception:" + ex);
            }

        }
    }
}
