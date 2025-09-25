namespace PAYGLoadFileToTable
{
    using PAYGLoadFileToTable.CompositionRoot;

    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<PAYGLoadFileToTableJob>();
            job.Execute();
        }
    }
}
