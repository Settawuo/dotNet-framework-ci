namespace FBBPayGCheckLoadFile
{
    using FBBPayGCheckLoadFile.CompositionRoot;

    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBBPayGCheckLoadFileJob>();
            job.Execute();
        }
    }
}
