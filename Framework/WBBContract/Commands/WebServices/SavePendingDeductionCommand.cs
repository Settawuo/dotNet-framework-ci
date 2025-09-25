
namespace WBBContract.Commands.WebServices
{
    public class SavePendingDeductionCommand
    {
        public SavePendingDeductionCommand()
        {
            this.ret_code = "-1";
            this.ret_message = "";
        }
        public string p_transaction_id { get; set; }
        public string p_mobile_no { get; set; }
        public string p_ba_no { get; set; }
        public string p_paid_amt { get; set; }
        public string p_channel { get; set; }
        public string p_merchant_id { get; set; }
        public string p_payment_method_id { get; set; }
        public string p_order_transaction_id { get; set; }
        // out
        public string ret_code { get; set; }
        public string ret_message { get; set; }
    }
}
