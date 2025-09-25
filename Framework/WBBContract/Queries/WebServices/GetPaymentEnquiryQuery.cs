using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPaymentEnquiryQuery : IQuery<GetPaymentEnquiryModel>
    {
        public string tranTime { get; set; }
    }
}
