using FBBPayGLastMileBydistanceBatch.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBPayGLastMileBydistanceBatch
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();

            var job = Bootstrapper.GetInstance<FBBPayGLastMileBydistanceBatchJob>();
              job.ManagerAsync();
        }
    }
}
