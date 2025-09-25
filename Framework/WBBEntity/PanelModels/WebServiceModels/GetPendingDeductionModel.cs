using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetPendingDeductionModel
    {
        public string ret_code { get; set; }
        public string ret_message { get; set; }
        public List<OrderPaendingDeduction> orderPaendingDeductionDatas { get; set; }
    }

    public class OrderPaendingDeduction
    {
        public string endpoint { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string pm_moblie_num { get; set; }
        public string pm_billing_acc_num { get; set; }
        public string pm_paid_amt { get; set; }
        public string pm_receipt_location { get; set; }
        public string pm_payment_channel_id { get; set; }
        public string pm_terminal_id { get; set; }
        public string pm_user_id { get; set; }
        public string pm_payment_method_id { get; set; }
        public string pm_print_flag { get; set; }
        public string pm_ais_ref { get; set; }
        public string pm_tran_id { get; set; }
        public string pm_sub_channel { get; set; }
        public string pm_bank_code { get; set; }
    }
}
