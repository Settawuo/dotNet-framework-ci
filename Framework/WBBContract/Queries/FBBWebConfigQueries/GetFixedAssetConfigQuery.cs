using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetFixedAssetConfigQuery : IQuery<List<LovModel>>
    {
        public string Program { get; set; }

    }
    public class GetFixedAssetResultQuery : IQuery<List<FixedAssetInventoryModel>>
    {

        public string p_order_type { get; set; }
        public string p_product_name { get; set; }
        public string p_service_name { get; set; }
        public string p_ord_dt_from { get; set; }
        public string p_ord_dt_to { get; set; }

    }
}
