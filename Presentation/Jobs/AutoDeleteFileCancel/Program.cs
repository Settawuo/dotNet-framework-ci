using AutoDeleteFileCancel.CompositionRoot;
using System;

namespace AutoDeleteFileCancel
{

    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBAutoDeleteFileCancelJob>();

            bool isSuccess = job.AutoDeleteFileCancel();
            Console.WriteLine("Execute AutoDeleteFileCancel");

        }
    }
}
