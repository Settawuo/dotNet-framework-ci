using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckPrivilegeQuery : IQuery<CheckPrivilegeQueryModel>
    {
        public string MobileNo { get; set; }
        public string ShortCodeLovName { get; set; }
        public string FullURL { get; set; }
    }

    public class CheckPrivilegePointQuery : IQuery<CheckPrivilegePointQueryModel>
    {
        public string PaymentOrderID { get; set; }
        public string MobileNo { get; set; }
        public string InternetNo { get; set; }
        public string FullURL { get; set; }
    }

    public class PrivilegeRedeemPointQuery : IQuery<PrivilegeRedeemPointQueryModel>
    {
        public string SFFPromotioncode { get; set; }
        public string PaymentOrderID { get; set; }
        public string MobileNo { get; set; }
        public string FullURL { get; set; }
    }
}
