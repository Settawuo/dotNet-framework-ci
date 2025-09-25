using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBBFBSSToPAYG
{
    using System.IO;
    using FBBFBSSToPAYG.CompositionRoot;
    using WBBBusinessLayer;

    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBBFBSSToPAYGJob>();
            job.Execute();
        }
    }
}
