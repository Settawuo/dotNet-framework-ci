using FBBPAYGLoadSIM.CompositionRoot;
using System;
using WBBEntity.Extensions;

namespace FBBPAYGLoadSIM
{
    internal class Program
    {
        private static string _errorMessage = "";

        private static void Main(string[] args)
        {
            //Bootstrapper.Bootstrap();
            //FBBPayG_LoadFile_3BBReportBatch instance = Bootstrapper.GetInstance<FBBPayG_LoadFile_3BBReportBatch>();
            //var temp = Bootstrapper.GetInstance<FBBPayG_LoadFile_3BBReportBatch>();
            //var logger = Bootstrapper.GetInstance<DebugLogger>();


            Bootstrapper.Bootstrap();

            //FBBPAYGLoadSIMBatch jobs = Bootstrapper.GetInstance<FBBPAYGLoadSIMBatch>();

            var logger = Bootstrapper.GetInstance<DebugLogger>();
            FBBPAYGLoadSIMBatch instance = Bootstrapper.GetInstance<FBBPAYGLoadSIMBatch>();
            //var temp = Bootstrapper.GetInstance<FBBPAYGLoadSIMBatch>();
            try
            {
                instance.ProcessLoadSIM();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At FBBPAYGLoadSIM");
                logger.Info(_errorMessage);
            }
            Console.WriteLine($"FBBPAYGLoadSIM End");
            Console.ReadLine();
        }
    }
}
