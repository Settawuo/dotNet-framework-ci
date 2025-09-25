using System.Collections.Generic;
namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetCreateMerchantQrCodeModel
    {
        public string RespCode { get; set; }
        public string RespDesc { get; set; }
        public string OrderId { get; set; }
        public string QrCodeStr { get; set; }
        public string QrFormat { get; set; }
        public string QrCodeValidity { get; set; }
        public string Reference { get; set; }
    }

    public class GetValueQrCodeModel
    {
        public string RespCode { get; set; }
        public string RespDesc { get; set; }
        public string PaymentOrderID { get; set; }
        public string PurchaseAmt { get; set; }
    }

    public class GetOrderChangeServiceModel
    {
        public string RespCode { get; set; }
        public string RespDesc { get; set; }
        public List<OrdChangeService> OrdChangeServiceList { get; set; }
        public List<OrdServiceAttribute> OrdServiceAttributeList { get; set; }
        public List<OrdFee> OrdFeeList { get; set; }

    }

    public class OrdChangeService
    {
        public string mobileNo { get; set; }
        public string orderChannel { get; set; }
        public string locationCd { get; set; }
        public string ascCode { get; set; }
        public string orderType { get; set; }
        public string userName { get; set; }
        public string referenceNo { get; set; }
        public string employeeID { get; set; }
        public string actionStatus { get; set; }
        public string serviceCode { get; set; }
    }

    public class OrdServiceAttribute
    {
        public string serviceAttributeName { get; set; }
        public string serviceAttributeValue { get; set; }
    }

    public class OrdFee
    {
        public string parameterType { get; set; }
        public string parameterName { get; set; }
        public string parameterValue { get; set; }
    }

    public class GetMeshCustomerProfileModel
    {
        public string ret_code { get; set; }
        public string ret_message { get; set; }
        public string order_list { get; set; }
        public string amount { get; set; }
        public string purchase_amt { get; set; }
        public string tran_id { get; set; }
        public string order_date { get; set; }
        public string customer_name { get; set; }
        public string non_mobile_no { get; set; }
        public string contact_mobile { get; set; }
        public string install_date { get; set; }
        public string language { get; set; }
        public string sff_promotion_code { get; set; }

        //R20.10 ref1
        public string ba { get; set; }
    }

    public class CreateOrderMeshPromotionResult
    {
        public string ret_code { get; set; }
        public string ret_message { get; set; }
        public string order_no { get; set; }
    }
}