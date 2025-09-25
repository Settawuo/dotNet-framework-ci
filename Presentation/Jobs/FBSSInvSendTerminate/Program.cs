using FBSSInvSendTerminate.CompositionRoot;

namespace FBSSInvSendTerminate
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBSSInvSendTerminateJob>();
            job.Execute();
        }
    }
}
