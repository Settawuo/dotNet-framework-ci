namespace DormPAYGReport
{
    using DormPAYGReport.CompositionRoot;

    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<DormPAYGReportJob>();

            //job.DeleteFileInFirstDateOfMonth();
            job.Execute();
            //job.SendMail();
        }
    }
}
