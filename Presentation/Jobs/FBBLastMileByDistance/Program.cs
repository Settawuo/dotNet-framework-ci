namespace FBBLastMileByDistance
{
    using FBBLastMileByDistance.CompositionRoot;
    class Program
    {

        static void Main(string[] args)
        {

            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBBLastMileByDistanceJob>();
            job.Execute();
        }
    }
}
