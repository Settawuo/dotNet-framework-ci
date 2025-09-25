using FBBSubmitFOALogSendMail.CompositionRoot;
using System;

namespace FBBSubmitFOALogSendMail
{
    internal class Program
    {

        private static void Main(string[] args)
        {

            #region Release 04.03.2021
            try
            {
                Bootstrapper.Bootstrap();
                FBBSubmitFOALogSendMailBatchJob jobs = Bootstrapper.GetInstance<FBBSubmitFOALogSendMailBatchJob>();
                if (jobs.CheckProcessBatch())
                {
                    Console.WriteLine($"Auto Resend start");
                    var PROCESS_INS = jobs.Get_FBB_CFG_LOV("FBB_SUBMITFOALOGSENDMAIL_BATCH", "PROCESS_INS");
                    if (PROCESS_INS != "Y")
                    {
                        jobs.AutoResendOrderNew();
                    }
                    else
                    {
                        jobs.AutoResendOrderNewR2105();
                    }
                    Console.WriteLine($"Auto Resend end");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR :{ex.Message.ToString()}");
            }
            #endregion

            #region Not Use Release 04.03.2021
            //Bootstrapper.Bootstrap();
            //int index = 1;
            //do
            //{
            //    using (FBBSubmitFOALogSendMailBatchJob jobs = Bootstrapper.GetInstance<FBBSubmitFOALogSendMailBatchJob>())
            //    {
            //        try
            //        {
            //            if (jobs.CheckProcessBatch())
            //            {
            //                Console.WriteLine($"Auto Resend start #{index}");
            //                var result = index != 1 ? jobs.AutoResendOrder(index.ToString()) : jobs.AutoResendOrder();
            //                Console.WriteLine($"Auto Resend end #{index}");
            //                Console.WriteLine($"Please wait...");
            //                if (!result && index > 1)
            //                    break;
            //            }
            //            index += 1;
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine($"ERROR :{ex.Message.ToString()}");
            //        }
            //        finally
            //        {
            //            jobs.Dispose();
            //            Thread.Sleep(TimeSpan.FromMinutes(10));
            //       }
            //    }
            //} while (index < 6);
            #endregion
        }

    }
}

