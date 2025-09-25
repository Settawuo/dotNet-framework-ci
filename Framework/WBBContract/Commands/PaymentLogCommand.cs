using System;

namespace WBBContract.Commands
{
    public class PaymentLogCommand
    {
        public decimal? PaymentId { get; set; }
        public string SessionId { get; set; }
        public string Browser { get; set; }
        public string CustInternetNum { get; set; }
        public string CustIdCardType { get; set; }
        public string CustIdCardNum { get; set; }
        public string DueDate { get; set; }
        public string RequestParamMode { get; set; }
        public string RequestParamPaymentMethod { get; set; }
        public string RequestParamMerchantId { get; set; }
        public string RequestParamServiceId { get; set; }
        public string RequestParamRef1 { get; set; }
        public string RequestParamRef2 { get; set; }
        public string RequestParamAmount { get; set; }
        public string RequestParamUsername { get; set; }
        public string RequestParamPassword { get; set; }
        public string RequestUrl { get; set; }
        public DateTime? RequestDatetime { get; set; }
        public string ResponseMoblieNo { get; set; }
        public string ResponseAmount { get; set; }
        public string ResponseReceiptNum { get; set; }
        public string ResponseRef1 { get; set; }
        public string ResponseTxid { get; set; }
        public string ResponseUrlForward { get; set; }
        public string ResponseUrl { get; set; }
        public string ResponseSessRef1 { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public string ResponseBalance { get; set; }
        public string ResponseValidity { get; set; }
        public string ResponseRef2 { get; set; }

        public string Status { get; set; }
        public DateTime? ResponseDatetime { get; set; }

        public string ReturnCode { get; set; }
        public string ReturnDesc { get; set; }


    }
}
