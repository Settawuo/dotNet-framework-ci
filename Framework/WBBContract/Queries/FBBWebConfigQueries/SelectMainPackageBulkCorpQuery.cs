using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectMainPackageBulkCorpQuery : IQuery<List<ReturnPackageList>>
    {
        public string AccntCat { get; set; }
    }
}
