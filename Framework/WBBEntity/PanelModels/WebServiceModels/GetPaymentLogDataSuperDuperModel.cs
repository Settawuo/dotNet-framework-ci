using WBBEntity.Models;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetPaymentLogDataSuperDuperModel
    {
        public FBB_REGISTER_PAYMENT_LOG_SPDP DataLog { get; set; }
        public string code { get; set; }
        public string message { get; set; }
    }
}