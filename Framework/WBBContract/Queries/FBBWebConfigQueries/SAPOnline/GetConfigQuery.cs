using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class FBSSConfigQuery : IQuery<FBSSConfig>
    {
        public string CON_TYPE { get; set; }
    }

}
