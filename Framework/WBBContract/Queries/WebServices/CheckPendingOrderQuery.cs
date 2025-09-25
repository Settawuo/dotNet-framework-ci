using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckPendingOrderQuery : IQuery<CheckPendingOrderModel>
    {
        public string AisNonMobile { get; set; }
        public string RegisterType { get; set; }
        public string FullUrl { get; set; }
    }
}
