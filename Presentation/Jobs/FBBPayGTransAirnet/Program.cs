using System;
using WBBEntity.Extensions;

namespace FBBPayGTransAirnet
{
    using FBBPayGTransAirnet.CompositionRoot;

    class Program
    {
        private static string _errorMessage = "";

        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var temp = Bootstrapper.GetInstance<FBBPayGTransAirnetJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                temp.ExecuteJob();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At FBBPayGTransAirnet");
                logger.Info(_errorMessage);
            }



        }
    }
}
