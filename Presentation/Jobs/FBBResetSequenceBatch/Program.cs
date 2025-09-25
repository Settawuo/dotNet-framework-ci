using FBBResetSequenceBatch.CompositionRoot;

namespace FBBResetSequenceBatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBResetSequenceJob>();
            job.Execute();
        }
    }
}
