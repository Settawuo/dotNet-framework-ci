using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPaymentToDeductionQuery : IQuery<GetPaymentToDeductionModel>
    {
        public string FullUrl { get; set; }
        public string UpdateBy { get; set; }
        public string InternetNo { get; set; }
        public string Amount { get; set; }
        public string BillingNo { get; set; }
        public string ProductName { get; set; }
        public string ServiceName { get; set; }
    }
}