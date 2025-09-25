using FBBDormitoryBatchRequestForService.CompositionRoot;

using One2NetBusinessLayer.QueryHandlers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBBDormitoryBatchRequestForService
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBBDormitoryBatchRequestForServiceJob>();
            job.Execute();
        }
    }
}
