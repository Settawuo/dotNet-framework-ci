using FBBGoodsMovementKAFKA;
using FBBGoodsMovementKAFKABatch.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.Extensions;

namespace FBBGoodsMovementKAFKABatch
{
    class Program
    {
        private static string _errorMessage = "";
        static void Main(string[] args)
        {
            //Console.WriteLine("Test FBBGoodsMovementKAFKABatch");
            //Console.ReadLine();

            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBBGoodsMovementKAFKAJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                logger.Info("Start At FBBGoodsMovementKAFKABatch");
                job.ExecuteJob();

                logger.Info("FBBGoodsMovementKAFKABatch Done");
            }
            catch (Exception ex)
            {
                _errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At FBBGoodsMovementKAFKABatch");
                logger.Info(_errorMessage);
            }
        }
    }
}
