namespace WBBContract.Commands.WebServices
{
    public class SaveDeductionReceiptLogCommand
    {
        public SaveDeductionReceiptLogCommand()
        {
            this.ret_code = "-1";
            this.ret_message = "";
        }
        public string p_user_name { get; set; }
        public string p_transaction_id { get; set; }
        public double p_pm_receipt_id { get; set; }
        public string p_pm_receipt_num { get; set; }
        public string p_pm_billing_acc_num { get; set; }
        public double p_pm_receipt_tot_mny { get; set; }
        public double p_pm_tax_mny { get; set; }
        public string ret_code { get; set; }
        public string ret_message { get; set; }
    }
}
