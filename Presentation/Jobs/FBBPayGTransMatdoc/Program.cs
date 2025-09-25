using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.Extensions;

namespace FBBPayGTransMatdoc
{
    using FBBPayGTransMatdoc.CompositionRoot;

    class Program
    {
        private static string _errorMessage = "";
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var temp = Bootstrapper.GetInstance<FBBPayGTransMatdocJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                temp.ExecuteJob();


            }
            catch (Exception ex)
            {
                _errorMessage = ex.RenderExceptionMessage();
                logger.Info("Error At FBBPayGTransMatdoc");
                logger.Info(_errorMessage);
            }
        }
    }
}
