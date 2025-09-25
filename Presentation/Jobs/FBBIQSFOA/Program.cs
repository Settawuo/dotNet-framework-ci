using FBBIQSFOA.CompositionRoot;
using System;
using WBBEntity.Extensions;

namespace FBBIQSFOA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBBIQSFOAJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                job.Execute();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At FBBIQSFOA");
                logger.Info(errorMessage);
            }

        }
    }
}


