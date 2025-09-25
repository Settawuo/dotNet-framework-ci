using FBBDelectInterfaceLogARCBatch.CompositionRoot;
using System;
using WBBEntity.Extensions;

namespace FBBDelectInterfaceLogARCBatch
{
    public class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var job = Bootstrapper.GetInstance<FBBDelectInterfaceLogARCBatchJob>();

            try
            {
                Console.WriteLine("Start Execute FBBDelectInterfaceLogARCBatch");
                job.Execute();
                Console.WriteLine("End Execute FBBDelectInterfaceLogARCBatch");
            }
            catch (Exception ex)
            {
                var errorMessage = ex.RenderExceptionMessage();
                Console.WriteLine("Error At FBBDelectInterfaceLogARCBatch : " + errorMessage);
            }
        }
    }
}
