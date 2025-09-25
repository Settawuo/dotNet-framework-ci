using AutoCreateFolderNas.CompositionRoot;
using System;

namespace AutoCreateFolderNas
{

    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBAutoCreateFolderNasBatchJob>();

            bool isSuccess = job.AutoCreateFolderNas();
            Console.WriteLine("Execute AutoCreateFolderNas");

        }
    }
}
