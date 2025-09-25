using FBBBulkCorpBatchService.CompositionRoot;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FBBBulkCorpBatchService
{
    public class Program
    {
        static void Main(string[] args)
        {

            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBBulkCorpBatchServiceJob>();
            BackgroundWorker backgroundWorker1 = new BackgroundWorker();

            backgroundWorker1.DoWork += (sender, e) =>
                {
                    job.SffReturn();
                    Console.WriteLine("Execute WorkFlowService");
                    job.ExecuteWorkFlowService();
                    Console.WriteLine("Execute SFF BA_NO,CA_NO,SA_NO Return");

                };

            BackgroundWorker backgroundWorker2 = new BackgroundWorker();
            backgroundWorker2.DoWork += (sender, e) =>
            {
                job.QueryDataToSendMail();
                Console.WriteLine("Execute send mail");
            };

            //BackgroundWorker backgroundWorker3 = new BackgroundWorker();
            //backgroundWorker3.DoWork += (sender, e) =>
            //{

            //};

            // schedule backgroundWorker1 and backgroundWorker2 to run in parallel
            Task<object> t1 = backgroundWorker1.ToTask();
            Task<object> t2 = backgroundWorker2.ToTask();
            //  Task<object> t3 = backgroundWorker3.ToTask();

            Task.WhenAll(t1, t2).Wait();

            // schedule backgroundWorker3 to run when both backgroundWorker1 
            // and backgroundWorker2 are completed
            //  Task<object> t3 = backgroundWorker3.ToTask();
            //  t3.Wait();
        }
    }
}
