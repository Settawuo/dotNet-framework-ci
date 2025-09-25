using FBBAutoMoveFileBatch.CompositionRoot;
using System;

namespace FBBAutoMoveFileBatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBAutoMoveFileBatchJob>();
            bool isSuccess = job.AutoMoveFile();
            Console.WriteLine("Execute auto move file batch");
            job.QueryDataToSendMail(isSuccess);
            Console.WriteLine("Execute send mail");
        }
    }
}
