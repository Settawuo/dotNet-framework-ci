using FBBDeleteFileOrderCancel.CompositionRoot;
using System;

namespace FBBDeleteFileOrderCancel
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBDeleteFileOrderCancelJob>();
            bool isSuccess = job.DeleteFileOrderCancel();
            Console.WriteLine("Execute Delete File Order Cancel");
            job.QueryDataToSendMail(isSuccess);
            Console.WriteLine("Execute send mail");
        }
    }
}
