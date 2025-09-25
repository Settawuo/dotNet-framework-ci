using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class PmModleDetailResponse
    {
        public string StatusDesc { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string InternetNo { get; set; }
        public string BillingNo { get; set; }
        public double TotalBalance { get; set; }
        public string DueDate { get; set; }
        public string TransactionId { get; set; }
    }

    public class ConfPMPayResponse
    {
        public double[] PM_RECEIPT_ID { get; set; }
        public string[] PM_RECEIPT_NUM { get; set; }
        public string[] PM_BILLING_ACC_NUM { get; set; }
        public double[] PM_RECEIPT_TOT_MNY { get; set; }
        public double[] PM_TAX_MNY { get; set; }
        public string PM_TUX_CODE { get; set; }
        public string PM_TUX_MSG { get; set; }
        public string PM_USER_ERR_MSG { get; set; }
        public string PM_SRV_ERROR { get; set; }
    }

    public class PaymentOutstandingbalConfigModel
    {
        public string Url { get; set; }
        public string UseSecurityProtocol { get; set; }
        public string ContentType { get; set; }
        public string Authorization { get; set; }
        public string ProjectCode { get; set; }
        public string BodyStr { get; set; }
    }

    public class PaymentOutstandingbalConfigBody
    {
        public string[] mobileList { get; set; }
        public string debtType { get; set; }
        public string invRespFlag { get; set; }
        public string orderRespFlag { get; set; }
        public string creditLimitRespFlag { get; set; }
        public string queryInactiveFlag { get; set; }
        public string orderGroup { get; set; }
        public string userId { get; set; }
    }

    public class PaymentOutstandingbalConfigResult
    {
        public List<PaymentOutstandingbalResponse> Response { get; set; }
        public string ErrorDesc { get; set; }
        public string ErrorMsg { get; set; }
        public string ErrorCode { get; set; }
    }

    public class PaymentOutstandingbalResponse
    {
        public string baNo { get; set; }
        public string baStatus { get; set; }
        public string excessPaymentMNY { get; set; }
        public string invoiceBalMNY { get; set; }
        public string totalBalMNY { get; set; }
        public string baCompany { get; set; }
        public string caNo { get; set; }
        public string mobileNo { get; set; }
        public string mobileStatus { get; set; }
        public string suspendCreditFlag { get; set; }
        public string payAmt { get; set; }
        public string overUsage { get; set; }
        public string minAdvPayment { get; set; }
        public List<PaymentOutstandingbalInvoice> invoiceList { get; set; }
    }

    public class PaymentOutstandingbalInvoice
    {
        public string billingAccount { get; set; }
        public string invoicingCoId { get; set; }
        public string invoiceNum { get; set; }
        public string invoiceType { get; set; }
        public string movementDat { get; set; }
        public string paymentDueDat { get; set; }
        public string billStartDat { get; set; }
        public string billEndDat { get; set; }
        public string invoiceIncVatMny { get; set; }
        public string invoiceExcVatMny { get; set; }
        public string invoiceVatMny { get; set; }
        public string invoiceNonVatMny { get; set; }
        public string invoiceTotalMny { get; set; }
        public string invoiceIncVatBalMny { get; set; }
        public string invoiceExcVatBalMny { get; set; }
        public string invoiceVatBalMny { get; set; }
        public string invoiceNonVatBalMny { get; set; }
        public string invoiceTotalBalMny { get; set; }
        public string monthlyFeeMny { get; set; }
        public string monthlyFeeWtRate { get; set; }
        public string serviceChargeMny { get; set; }
        public string serviceChargeWtRate { get; set; }
        public string billCycle { get; set; }
        public string billCycleNum { get; set; }
        public string partialPaidFlag { get; set; }

    }
}
