using System;
using WBBEntity.Extensions;

namespace FBBInventoryReconcile
{
    using FBBInventoryReconcile.CompositionRoot;

    class Program
    {
        private static string _errorMessage = "";

        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBBInventoryReconcileJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                logger.Info("Start At FBBInventoryReconcile");
                job.ExecuteJob();
            }
            catch (Exception ex)
            {
                _errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At FBBInventoryReconcile");
                logger.Info(_errorMessage);
            }

        }
    }
}
