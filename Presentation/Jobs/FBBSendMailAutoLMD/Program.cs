using FBBSendMailAutoLMD.CompositionRoot;
using System;

namespace FBBSendMailAutoLMD
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("FBBSendMailAutoLMD Program running.");
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<SendMailAutoLMD>();
            job.ManagerAsync();
        }
    }
}
