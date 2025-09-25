using QueryDataFBBDashboard.CompositionRoot;
using System;
using WBBEntity.Extensions;

namespace QueryDataFBBDashboard
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<QueryDataFBBDashboardJob>();

            try
            {
                Console.WriteLine("Start Execute QueryDataFBBDashboard");
                job.Execute();
                Console.WriteLine("End Execute QueryDataFBBDashboard");
            }
            catch (Exception ex)
            {
                var errorMessage = ex.RenderExceptionMessage();
                Console.WriteLine("Error At QueryDataFBBDashboard : " + errorMessage);
            }
        }
    }
}
