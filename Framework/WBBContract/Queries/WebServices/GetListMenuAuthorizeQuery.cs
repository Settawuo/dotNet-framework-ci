using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class GetListMenuAuthorizeQuery : IQuery<MenuAuthorizeModel>
    {
        public string P_PARTNER_TYPE { get; set; }
        public string P_PARTNER_SUBTYPE { get; set; }
    }

    public class PackageTopupInternetNotUseQuery : IQuery<PackageTopupInternetNotUseModel>
    {
        public string NonMobileNo { get; set; }
        public List<CurrentPromotionData> ListCurrentPromotion { get; set; }
    }

    public class CurrentPromotionData
    {
        public string product_class { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string product_cd { get; set; }
        public string product_status { get; set; }
    }
}
