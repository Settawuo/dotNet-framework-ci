namespace WBBContract.Queries.WebServices
{
    public class DeductionUpddateSendSMSFlagQuery : IQuery<string>
    {
        public string p_transaction_id { get; set; }
        public string p_nonmobile_no { get; set; }
        public string p_sms_flag { get; set; }
        public string p_update_by { get; set; }
    }
}
