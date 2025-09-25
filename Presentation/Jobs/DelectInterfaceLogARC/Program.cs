using DelectInterfaceLogARC.CompositionRoot;
using System;
using WBBEntity.Extensions;

namespace DelectInterfaceLogARC
{
    public class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<DelectInterfaceLogARCJob>();

            try
            {
                Console.WriteLine("Start Execute DelectInterfaceLogARC");
                var result = job.Execute();

                if (result != "")
                {
                    Console.WriteLine("Start send mail");
                    job.QueryDataToSendMail(result);
                    Console.WriteLine("End send mail");
                }

                Console.WriteLine("End Execute DelectInterfaceLogARC");
            }
            catch (Exception ex)
            {
                var errorMessage = ex.RenderExceptionMessage();
                Console.WriteLine("Error At DelectInterfaceLogARC : " + errorMessage);
            }
        }
    }
}
