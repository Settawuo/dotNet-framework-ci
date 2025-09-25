using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPaymentInquiryToSuperDuperQuery : IQuery<GetPaymentInquiryToSuperDuperModel>
    {
        public string Url { get; set; }

        //Header
        public string ContentType { get; set; }
        public string MerchantID { get; set; }
        public string Signature { get; set; }
        public string Nonce { get; set; }

        //Body
        public PaymentInquiryToSuperDuperBody Body { get; set; }
    }

    public class PaymentInquiryToSuperDuperBody
    {
        public string service_id { get; set; }
        public string card_ref { get; set; }
    }
}
