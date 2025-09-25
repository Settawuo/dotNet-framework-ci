using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetServicePlayboxMappingCodeModel
    {
        public int RETURN_CODE { get; set; }
        public string RETURN_DESC { get; set; }
        public string GOTO_TOPUP { get; set; }
        public List<ServicePlayboxMappingCodeModel> LIST_DATA { get; set; }
    }

    public class ServicePlayboxMappingCodeModel
    {
        public string SERVICE_PLAYBOX { get; set; }
        public string SFF_PROMOTION_CODE { get; set; }
        public string SFF_PRODUCT_NAME { get; set; }
        public decimal PRICE_CHARGE { get; set; }
        public string SFF_PRODUCT_CLASS { get; set; }
    }
}
