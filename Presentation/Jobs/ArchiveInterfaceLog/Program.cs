namespace ArchiveInterfaceLog
{
    using ArchiveInterfaceLog.CompositionRoot;

    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<ArchiveInterfaceLogJob>();
            job.Execute();
        }
    }
}
