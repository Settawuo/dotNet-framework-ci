using FBBAutoMoveFileByDayBatch.CompositionRoot;
using System;
namespace FBBAutoMoveFileByDayBatch
{
    class Program
    {
        static void Main(string[] args)
        {

            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBAutoMoveFileByDayBatchJob>();
            bool isSuccess = job.AutoMoveFileByDay();
            Console.WriteLine("Execute auto move file By Day batch");
            job.QueryDataToSendMail(isSuccess);
            Console.WriteLine("Execute send mail move file By Day");
        }
    }
}
