using FBSSFixedOM010SendMailFileLogBatch.CompositionRoot;

namespace FBSSFixedOM010SendMailFileLogBatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBSSFixedOM010SendMailFileLogBatchJob>();
            job.Execute();
        }
    }
}
