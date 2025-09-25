using FBBDeleteTransactionLogBatch.CompositionRoot;

namespace FBBDeleteTransactionLogBatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBDeleteTransactionLogJob>();
            job.Execute();
        }
    }
}
