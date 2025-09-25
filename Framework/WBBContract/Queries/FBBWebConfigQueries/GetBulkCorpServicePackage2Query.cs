using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetBulkCorpServicePackage2Query : IQuery<List<ReturnServicePackageList>>
    {
        public string AccntCat { get; set; }
    }
}
