using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetListDeviceContractQuery : IQuery<List<ContractDeviceModel>>
    {
        public string TransactionID { get; set; }
        public string FullUrl { get; set; }
        public string P_CHANNEL { get; set; }
        public string P_EVENT { get; set; }
        public string P_PRODUCT_SUBTYPE { get; set; } //R22.07
        public string P_OWNER_PRODUCT { get; set; } //R22.07
        public string P_SALE_CHANNEL { get; set; } //R22.07
        public List<SFF_PROMOTION_CODE_DEVICE_CONTRACT> LIST_SFF_PROMOTION_CODE { get; set; }
    }

    public class SFF_PROMOTION_CODE_DEVICE_CONTRACT
    {
        public string SFF_PROMOTION_CODE { get; set; }
    }
}
