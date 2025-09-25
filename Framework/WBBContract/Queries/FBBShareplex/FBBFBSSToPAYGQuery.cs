using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBBEntity.PanelModels.ShareplexModels;

namespace WBBContract.Queries.FBBShareplex
{
    public class FBBFBSSToPAYGQuery : IQuery<FBBFBSSToPAYGModel>
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }
}