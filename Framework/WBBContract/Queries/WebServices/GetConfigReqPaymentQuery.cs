using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetConfigReqPaymentQuery : IQuery<GetConfigReqPaymentModel>
    {
        public string p_product_name { get; set; }
        public string p_service_name { get; set; }
        public string p_transaction_id { get; set; }
        public string p_non_mobile_no { get; set; }
    }
}
