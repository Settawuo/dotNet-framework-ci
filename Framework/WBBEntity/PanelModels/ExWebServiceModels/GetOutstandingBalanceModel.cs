using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class GetOutstandingBalanceModel
    {
        public string ErrorMsg { get; set; }
        public string ErrorDesc { get; set;}
        public string ErrorCode { get; set; }
        public List<OutstandingbalResponse> Response { get; set; }
    }
    public class OutstandingbalResponse
    {
        public string baNo { get; set; }
        public string baStatus { get; set; }
        public double excessPaymentMNY { get; set; }
        public double invoiceBalMNY { get; set; }
        public double orderBalMNY { get; set; }
        public double totalBalMNY { get; set; }
        public string baCompany { get; set; }
        public string caNo { get; set; }
        public string mobileNo { get; set; }
        public string mobileStatus { get; set; }
        public string suspendCreditFlag { get; set; }
        public double minAdvPayment { get; set; }
        public double payAmt { get; set; }
        public double overUsage { get; set; }
        
        public List<OutstandingbalInvoice> invoiceList { get; set; }
        public List<OutstandingbalorderList> orderList { get; set; }
        public double pendingRequestMny { get; set; }
        public string baAccRef { get; set; }
        public string baNameMasking { get; set; }
    }
    public class OutstandingbalInvoice
    {
        public string billingAccount { get; set; }
        public string invoicingCoId { get; set; }
        public string invoiceNum { get; set; }
        public string invoiceType { get; set; }
        public string movementDat { get; set; }
        public string paymentDueDat { get; set; }
        public string billStartDat { get; set; }
        public string billEndDat { get; set; }
        public double invoiceIncVatMny { get; set; }
        public double invoiceExcVatMny { get; set; }
        public double invoiceVatMny { get; set; }
        public double invoiceNonVatMny { get; set; }
        public double invoiceTotalMny { get; set; }
        public double invoiceIncVatBalMny { get; set; }
        public double invoiceExcVatBalMny { get; set; }
        public double invoiceVatBalMny { get; set; }
        public double invoiceNonVatBalMny { get; set; }
        public double invoiceTotalBalMny { get; set; }
        public double monthlyFeeMny { get; set; }
        public double monthlyFeeWtRate { get; set; }
        public double serviceChargeMny { get; set; }
        public double serviceChargeWtRate { get; set; }
        public string billCycle { get; set; }
        public double billCycleNum { get; set; }
        public string partialPaidFlag { get; set; }
        public string invoiceDate { get; set; }

    }
    public class OutstandingbalorderList
    {
        public string orderRowId { get; set; }
        public string orderNumber { get; set; }
        public string orderDateTime { get; set; }
        public string orderSaleOrderType { get; set; }
        public string itemServiceNum { get; set; }
        public double orderBalMny { get; set; }
        public string partialPaidFlag { get; set; }
        public string xPromoType { get; set; }
        public string descText { get; set; }
        public double inclueVatMny { get; set; }
        public double discountBalanceAmt { get; set; }

        public double itemBasePrice { get; set; }
        public double itemDiscountAmt { get; set; }
        public double vatMny { get; set; }
    } 
    public class OutstandingbalConfigBody
    {
        //public List<string> baNoList { get; set; }
        //public List<string> mobileList { get; set; }
        //public List<string> billCycleList { get; set; }
        public List<string> baNoList { get; set; }
        public List<string> mobileList { get; set; }
        public string billCycleList { get; set; }
        public string debtType { get; set; }
        public string queryInactiveFlag { get; set; }
        public string invRespFlag { get; set; }
        public string orderRespFlag { get; set; }
        public string creditLimitRespFlag { get; set; }
        public string userId { get; set; }
        public string orderGroup { get; set; }
        public List<string> invoiceNoList { get; set; }
    }
}
