using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetFoawriteoffQuery : IQuery<List<FBSSFixedAssetSnAct>>
    {
        public string p_access_number { get; set; }
        public string p_serialNumber { get; set; }

        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
