using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class GetListStatusLeaveMessageQuery : IQuery<ListStatusLeaveMessageResponse>
    {
        public string CustName { get; set; }

        public string CustSurname { get; set; }

        public string ContactMobile { get; set; }

        public string RefferenceNo { get; set; }
    }
}
