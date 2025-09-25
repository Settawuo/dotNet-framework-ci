using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetStorageSubContractWfmQuery : IQuery<List<LovModel>>
    {
        public string p_storage_location { get; set; }
    }
}
