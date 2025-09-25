using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetCheckPromotionTopupReplaceQuery : IQuery<CheckPromotionTopupReplaceModel>
    {
        public string TransactionId { get; set; }
        public string FullUrl { get; set; }
        public string P_FLAG_LANG { get; set; }
        public List<ContractDeviceArrayModel> SffPromotionCodeList { get; set; }
    }

    public class ContractDeviceArrayModel
    {
        public string PROMOTION_CODE { get; set; }
    }
}
