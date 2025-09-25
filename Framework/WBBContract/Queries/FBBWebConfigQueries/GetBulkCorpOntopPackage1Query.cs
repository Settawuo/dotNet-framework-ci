using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetBulkCorpOntopPackage1Query : IQuery<List<ReturnOntopPackageList>>
    {
        public string AccntCat { get; set; }
    }
}
