using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetPaymentEnquiryModel
    {
        public string ret_code { get; set; }
        public string ret_message { get; set; }
        public List<OrderDeductEnquiry> OrderDeductEnquiryDatas { get; set; }
    }

    public class OrderDeductEnquiry
    {
        public string endpoint { get; set; }
        public string channel_secret { get; set; }
        public string non_mobile_no { get; set; }
        public string merchant_id { get; set; }
        public string txn_id { get; set; }
    }
}
