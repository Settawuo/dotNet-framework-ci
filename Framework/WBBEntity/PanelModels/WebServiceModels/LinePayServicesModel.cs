using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class LinePayOptionsModel
    {
        public List<LinePayOptionsAddFriendsModel> addFriends { get; set; }
        public LinePayOptionsShippingModel shipping { get; set; }
        public LinePayOptionsPaymentModel payment { get; set; }
    }

    public class LinePayOptionsAddFriendsModel
    {
        public string type { get; set; }
        public List<string> idList { get; set; }
    }

    public class LinePayOptionsShippingModel
    {
        public string type { get; set; }
        public string feeInquiryUrl { get; set; }
        public string feeInquiryType { get; set; }
    }

    public class LinePayOptionsPaymentModel
    {
        public string payType { get; set; }
    }

    public class LinePayPackagesModel
    {
        public string id { get; set; }
        public float amount { get; set; }
        public List<LinePayProductsModel> products { get; set; }
    }

    public class LinePayProductsModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string imageUrl { get; set; }
        public int quantity { get; set; }
        public float price { get; set; }
    }

    public class LinePayRedirectUrlsModel
    {
        public string confirmUrl { get; set; }
        public string cancelUrl { get; set; }
    }

    public class LinePayGeneralPaymentModel
    {
        public float amount { get; set; }
        public string currency { get; set; }
        public string orderId { get; set; }
        public List<LinePayPackagesModel> packages { get; set; }
        public LinePayRedirectUrlsModel redirectUrls { get; set; }
    }

    public class LinePayCheckoutPaymentModel
    {
        public float amount { get; set; }
        public string currency { get; set; }
        public string orderId { get; set; }
        public List<LinePayPackagesModel> packages { get; set; }
        public LinePayRedirectUrlsModel redirectUrls { get; set; }
        public LinePayOptionsModel options { get; set; }
    }

    public class LinePayAutomaticPaymentModel
    {
        public float amount { get; set; }
        public string currency { get; set; }
        public string orderId { get; set; }
        public List<LinePayPackagesModel> packages { get; set; }
        public LinePayRedirectUrlsModel redirectUrls { get; set; }
        public LinePayOptionsModel options { get; set; }
    }

    public class LinePayResponseModel
    {
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public LinePayInfoModel info { get; set; }
    }

    public class LinePayInfoModel
    {
        public string transactionId { get; set; }
        public LinePayPaymentUrlModel paymentUrl { get; set; }
        public string paymentAccessToken { get; set; }
    }

    public class LinePayPaymentUrlModel
    {
        public string web { get; set; }
        public string app { get; set; }
    }
}
