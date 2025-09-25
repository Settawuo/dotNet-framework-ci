using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class ChangeInstallDateQuery : IQuery<ChangeInstallDateModel>
    {
        public string P_ORDER_NO { get; set; }
        public string P_ID_CARD { get; set; }
        public string P_MOBILE_NO { get; set; }
        public string P_NON_MOBILE_NO { get; set; }
        public string P_INSTALL_DATE { get; set; }
        public string P_TIME_SLOT { get; set; }
    }
}
