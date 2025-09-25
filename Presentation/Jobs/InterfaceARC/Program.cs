namespace InterfaceARC
{
    using InterfaceARC.CompositionRoot;

    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<InterfaceARCJob>();
            job.Execute();
        }
    }
}
