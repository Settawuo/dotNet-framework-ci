using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckPendingOrderFbssQuery : IQuery<CheckPendingOrderFbssModel>
    {
        public string InteretNo { get; set; }
        public string FullUrl { get; set; }
    }
}
