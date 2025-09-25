using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;


namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetBulkCorpMainPackageQuery : IQuery<List<ReturnPackageList>>
    {
        public string AccntCat { get; set; }
        public string Techno { get; set; }
        public string PackCode { get; set; }
    }
}
