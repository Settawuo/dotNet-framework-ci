namespace FBBPayGReconcileCPE
{
    using FBBPayGReconcileCPE.CompositionRoot;

    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var temp = Bootstrapper.GetInstance<FBBPayGReconcileCPEJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            temp.ExecuteJob();

        }
    }
}
