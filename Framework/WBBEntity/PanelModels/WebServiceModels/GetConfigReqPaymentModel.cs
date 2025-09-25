using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetConfigReqPaymentModel
    {
        public string ret_code { get; set; }
        public string ret_message { get; set; }
        public List<ConfigReqPaymentData> list_config_req_payment { get; set; }
        public List<ConfigReqPaymentData> list_req_payment_metadata { get; set; }
        public List<ConfigReqPaymentData> list_req_payment_3ds { get; set; }
    }

    public class ConfigReqPaymentData
    {
        public string attr_name { get; set; }
        public string attr_value { get; set; }
    }

}
