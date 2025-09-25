using System;
using WBBEntity.Extensions;

namespace FBBCustInstallAddress
{
    using FBBCustInstallAddress.CompositionRoot;

    class Program
    {
        private static string _errorMessage = "";

        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var temp = Bootstrapper.GetInstance<FBBCustInstallAddressJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                logger.Info("Start At FBBCustInstallDelete");
                temp.ExecuteJob_new();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At FBBCustInstallDelete");
                logger.Info(_errorMessage);
            }

            //try
            //{
            //    logger.Info("Start At FBBCustInstallAddress");
            //    temp.ExecuteJob();
            //}
            //catch (Exception ex)
            //{
            //    _errorMessage = ex.RenderExceptionMessage();
            //    logger.Info("Error At FBBCustInstallAddress");
            //    logger.Info(_errorMessage);
            //}
        }
    }
}
