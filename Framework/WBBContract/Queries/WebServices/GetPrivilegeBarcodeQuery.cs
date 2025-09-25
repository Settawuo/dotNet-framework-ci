using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPrivilegeBarcodeQuery : IQuery<GetPrivilegeBarcodeModel>
    {
        public string UrlReqPrivilegeBarcode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }
        public string MobileNo { get; set; }
        public string ShortCode { get; set; }

        public string IDCardNo { get; set; }
        public string ClientIP { get; set; }
        public string FullURL { get; set; }
    }
}
