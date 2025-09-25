namespace FBBTrackingCompetitor
{
    using FBBTrackingCompetitor.CompositionRoot;

    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBBTrackingCompetitorJob>();

            job.Execute();
        }
    }
}
