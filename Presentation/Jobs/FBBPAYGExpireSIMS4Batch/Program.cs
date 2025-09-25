using FBBPAYGExpireSIMS4Batch.CompositionRoot;
using System;

namespace FBBPAYGExpireSIMS4Batch
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Bootstrapper.Bootstrap();
                FBBPAYGExpireSIMS4Job jobs = Bootstrapper.GetInstance<FBBPAYGExpireSIMS4Job>();
                Console.WriteLine($"FBBPAYGExpireSIM_S4 Start");

                //STEP1-CHECK-DIRECTORYs
                var result_directory = jobs.CheckBatchDirectory();
                Console.WriteLine($"FBBPAYGExpireSIM_S4: Directory found "+ result_directory + "");

                //STEP2-CHECK-FILENAME
                var result_filename = jobs.CheckBatchFilename();
                Console.WriteLine($"FBBPAYGExpireSIM_S4: Filename sample found " + result_filename + "");

                //STEP2.5-CHECK-PROCESS
                var result_process = jobs.CheckProcessStart();
                Console.WriteLine($"FBBPAYGExpireSIM_S4: PROCESS ACTIVE FLAG = " + result_process + "");

                //Check-Process-Flag
                if(result_process == "Y")
                {
                    //Check-Directory-and-filename-result
                    if (result_directory != "" && result_filename != "")
                    {
                        //STEP3-GET-DATA-AND-GEN-FILE
                        jobs.MainGenFiles(result_directory, result_filename);
                    }
                    else
                    {
                        Console.WriteLine($"!!ERROR DIRECTORY OR FILENAME!!");
                    }
                }
                else
                {
                    Console.WriteLine($"FBBPAYGExpireSIM_S4 ACTIVE Flag OFF (N)");
                }
                
                

                Console.WriteLine($"FBBPAYGExpireSIM_S4 End");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR :{ex.Message.ToString()}");
            }
        }
    }
}
