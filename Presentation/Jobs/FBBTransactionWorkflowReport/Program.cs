using FBBTransactionWorkflowReport.CompositionRoot;

namespace FBBTransactionWorkflowReport
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBTransactionWorkflowReportJob>();
            job.Execute();
        }
    }
}
