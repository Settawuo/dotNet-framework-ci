namespace CoverageAreaLog
{
    using CoverageAreaLog.CompositionRoot;

    public class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<CoverageAreaLogJob>();

            job.Execute();
        }
    }
}
