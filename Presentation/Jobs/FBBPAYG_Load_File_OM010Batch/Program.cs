using FBBPAYG_Load_File_OM010Batch.CompositionRoot;

namespace FBBPAYG_Load_File_OM010Batch
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBBPAYG_Load_File_OM010BatchJob>();
            job.Execute();

        }
    }
}
