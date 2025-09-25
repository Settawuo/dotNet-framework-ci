using FBBQueryOrderPendingDeduction.CompositionRoot;

namespace FBBQueryOrderPendingDeduction
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBQueryOrderPendingDeductionJob>();
            job.LogMsg("FBBQueryOrderPendingDeduction Start");
            job.StartWatching();

            job.QueryOrderPendingDeduction();

            job.StopWatching("Mode");
        }
    }
}
