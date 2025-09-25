using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class GetListPmMobileDetialQuery : IQuery<PmModleDetailResponse>
    {
        public string InternetNo { get; set; }
    }

    public class ConfPMPayQuery : IQuery<ConfPMPayResponse>
    {
        public string Url { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        public string PM_MOBLIE_NUM { get; set; }
        public string PM_BILLING_ACC_NUM { get; set; }
        public string[] PM_STATUS_CD { get; set; }
        public double PM_PAID_AMT { get; set; }
        public double PM_TAX_AMT { get; set; }
        public long PM_RECEIPT_LOCATION { get; set; }
        public long PM_PAYMENT_CHANNEL_ID { get; set; }
        public long PM_SHIFT_NUM { get; set; }
        public string PM_TERMINAL_ID { get; set; }
        public string PM_USER_ID { get; set; }
        public string PM_NEXT_BILL_FLAG { get; set; }
        public double PM_PAYMENT_METHOD_ID { get; set; }
        public long PM_PRINT_FLAG { get; set; }
        public string PM_TRAN_ID { get; set; }
        public string User { get; set; }
        public string PM_BANK_CODE { get; set; }
        public string PM_SUB_CHANNEL { get; set; }
        public string PM_ORDER_TRANSACTION_ID { get; set; }
        public string FullUrl { get; set; }
    }
}
