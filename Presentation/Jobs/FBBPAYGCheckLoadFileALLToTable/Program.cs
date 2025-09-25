using FBBPAYGCheckLoadFileALLToTable.CompositionRoot;

namespace FBBPAYGCheckLoadFileALLToTable
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBBPAYGCheckLoadFileALLToTableJob>();
            job.Execute();
        }
    }
}
