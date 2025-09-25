namespace QueryBuildingVillage
{
    using QueryBuildingVillage.CompositionRoot;

    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<QueryBuildingVillageJob>();
            job.Execute();
        }
    }
}
