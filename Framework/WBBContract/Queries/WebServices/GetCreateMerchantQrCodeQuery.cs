using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetCreateMerchantQrCodeQuery : IQuery<GetCreateMerchantQrCodeModel>
    {
        //Header
        public string ContentType { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }

        //Body
        public MerchantQrCodeBody Body { get; set; }
    }

    public class MerchantQrCodeBody
    {
        public string orderId { get; set; }
        public string channel { get; set; }
        public string serviceId { get; set; }
        public string terminalId { get; set; }
        public string locationName { get; set; }
        public string amount { get; set; }
        public string qrType { get; set; }
        public string ref1 { get; set; }
        public string ref2 { get; set; }
        public string ref3 { get; set; }
        public string ref4 { get; set; }
        public string ref5 { get; set; }
    }

    public class GetValueQrCodeQuery : IQuery<GetValueQrCodeModel>
    {
        public string InternetNo { get; set; }
    }

    public class GetOrderChangeServiceQuery : IQuery<GetOrderChangeServiceModel>
    {
        public string p_internet_no { get; set; }
        public string p_payment_order_id { get; set; }
    }

    public class GetMeshCustomerProfileQuery : IQuery<GetMeshCustomerProfileModel>
    {
        public string p_internet_no { get; set; }
        public string p_payment_order_id { get; set; }
    }

    public class MeshSmsQuery : IQuery<string>
    {
        public string mobileNo { get; set; }
        public string PurchaseAmt { get; set; }
        public string InstallDate { get; set; }
        public string NonMobileNo { get; set; }
        public string TranID { get; set; }
        public string MsgWay { get; set; }
        public string UrlForSentSMS { get; set; }
        public string FullUrl { get; set; }
        public bool IsThaiCulture { get; set; }
        public string Transaction_Id { get; set; }
        public string Source_Addr { get; set; }
        public string return_status { get; set; }
    }
}