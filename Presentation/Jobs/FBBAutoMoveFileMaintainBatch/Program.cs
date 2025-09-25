using FBBAutoMoveFileMaintainBatch.CompositionRoot;
using System;

namespace FBBAutoMoveFileMaintainBatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBAutoMoveFileMaintainBatchJob>();
            bool isSuccess = job.AutoMoveFile();
            Console.WriteLine("Execute auto move file maintain batch");
            job.QueryDataToSendMail(isSuccess);
            Console.WriteLine("Execute send mail");
        }
    }
}
