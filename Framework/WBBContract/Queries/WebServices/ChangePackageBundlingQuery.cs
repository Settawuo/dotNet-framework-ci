using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class ChangePackageBundlingQuery : IQuery<ChangePackageBundlingData>
    {
        public string P_NON_MOBILE { get; set; }
        public string P_MOBILE { get; set; }
        public string P_PRODUCT_MAIN_CD { get; set; }
        public string P_PROMOTION_ONTOP_CD { get; set; }
        public string P_FLAG_DISCOUNT { get; set; }
        public string FullUrl { get; set; }
    }
}
