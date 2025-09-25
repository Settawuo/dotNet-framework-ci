using System;

namespace FBBPayGTransAirnetRepair
{
    using FBBPayGTransAirnetRepair.CompositionRoot;
    using WBBEntity.Extensions;

    class Program
    {
        private static string _errorMessage = "";

        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var temp = Bootstrapper.GetInstance<FBBPayGTransAirnetRepairJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                temp.ExecuteJob();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At FBBPayGTransAirnetRepairJob");
                logger.Info(_errorMessage);
            }

        }
    }
}
