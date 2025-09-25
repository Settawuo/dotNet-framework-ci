using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetCheckPremiumInstallQuery : IQuery<CheckPremiumInstallModel>
    {
        public string p_order_no { get; set; }
    }
}
