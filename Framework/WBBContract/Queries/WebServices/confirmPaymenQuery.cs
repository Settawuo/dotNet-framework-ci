using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class confirmPaymenQuery : IQuery<ConfPMPayResponse>
    {
        public string User { get; set; }
        public string Url { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string fullUrl { get; set; }
        public string transaction_id { get; set; }
        public confirmPaymenBodyQuery bodyQuery { get; set; }
    }
    public class confirmPaymenBodyQuery
    {
        public long location { get; set; }
        public long paymentChannel { get; set; }
        public string tranId { get; set; }
        public string tranMode { get; set; }
        public string smsFlag { get; set; }
        public long channelGroupId { get; set; }
        public long settleSystemId { get; set; }
        public string settleCompany { get; set; }
        public string subChannel { get; set; }
        public double paidAmt { get; set; }
        public double wtAmt { get; set; }
        public string keyRec1 { get; set; }
        public string keyRec1Desc { get; set; }
        public string keyRec2 { get; set; }
        public string keyRec2Desc { get; set; }
        public long shiftNum { get; set; }
        public string terminalId { get; set; }
        public string userId { get; set; }
        public string paymentDate { get; set; }
        public string mobileOptional { get; set; }
        public string mobileSMS { get; set; }
        public double adjAmt { get; set; }
        public string paymentModel { get; set; }
        public string serviceId { get; set; }
        public string displayMobileFlag { get; set; }
        public List<payInfo> payInfo { get; set; }
        public List<method> method { get; set; }
    }
    public class payInfo
    {
        public string baNum { get; set; }
        public string mobileNum { get; set; }
        public string debtType { get; set; }
        public double paidAmt { get; set; }
        public double excessAmt { get; set; }
        public List<invoice> invoice { get; set; }
        public List<order> order { get; set; }

    }
    public class invoice
    {
        public string invoiceNum { get; set; }
        public double paidAmt { get; set; }


    }
    public class order
    {
        public string orderNum { get; set; }
        public string rowId { get; set; }
        public double paidAmt { get; set; }
    }
    public class method
    {
        public string methodId { get; set; }
        public int bankCode { get; set; }
        public double branchCode { get; set; }
        public string cardType { get; set; }
        public string docNum { get; set; }
        public string docDate { get; set; }
        public double paidAmt { get; set; }
        public string creditCardRefId { get; set; }
        public string bankRefId { get; set; }
        public string cardTxId { get; set; }
    }
}
