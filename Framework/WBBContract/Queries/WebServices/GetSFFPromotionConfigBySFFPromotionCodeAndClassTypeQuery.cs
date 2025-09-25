using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQuery : IQuery<List<GetSFFPromotionConfigBySFFPromotionCodeAndClassTypeQueryData>>
    {
        public string p_SFF_PROMOTION_CODE { get; set; }
        public string p_PROMOTION_CLASS { get; set; }
    }
}
