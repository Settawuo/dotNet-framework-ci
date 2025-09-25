using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPaymentEnquiryToSuperDuperQuery : IQuery<GetPaymentEnquiryToSuperDuperModel>
    {
        public string User { get; set; }
        public string Url { get; set; }
        public string p_transaction_id { get; set; }
        public string p_mobile_no { get; set; }
        public string Secret { get; set; }
        public string p_order_id { get; set; }


        //Header
        public string ContentType { get; set; }
        public string MerchantID { get; set; }
        public string Signature { get; set; }
        public string Nonce { get; set; }

        //Body
        public PaymentEnquiryToSuperDuperBody Body { get; set; }
        public string BodyStr { get; set; }
    }

    public class PaymentEnquiryToSuperDuperBody
    {
        public string txn_id { get; set; }
    }

}
