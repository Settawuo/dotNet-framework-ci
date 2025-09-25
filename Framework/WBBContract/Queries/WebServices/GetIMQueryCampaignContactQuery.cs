using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetIMQueryCampaignContactQuery : IQuery<GetIMQueryCampaignContactModel>
    {
        public string TransactionId { get; set; }
        public string FullUrl { get; set; }
        public string p_queryoption { get; set; }
        public string p_mobilenumber { get; set; }
        public string p_childcampaigncode { get; set; }
        public string p_inparameter1 { get; set; }
    }
}
