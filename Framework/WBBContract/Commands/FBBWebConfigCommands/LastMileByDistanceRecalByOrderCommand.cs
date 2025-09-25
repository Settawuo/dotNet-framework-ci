using System.Collections.Generic;
//using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class LastMileByDistanceRecalByOrderCommand
    {
        public List<LastMileRecal> p_recal_access_list { get; set; }
        public string p_NEW_RULE_ID { get; set; }
        public string p_USER { get; set; }
        public string p_REMARK { get; set; }

        //return 
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public List<ReturnCurRecalModel> return_subpayment_cur { get; set; }
    }

    //R19.03 Re cal distance
    public class LastMileRecal
    {
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_NO { get; set; }
        public string NEW_RULE_ID { get; set; }
        public string REMARK { get; set; }
        public string DISTANCE { get; set; }
        public string FLAG_RECAL { get; set; }
        public string REASON { get; set; }
    }
    //R19.03  End Re cal distance
    public class ReturnCurRecalModel
    {
        public string access_number { get; set; }
        public string order_no { get; set; }
        public decimal distance_to_paid { get; set; }
        public decimal total_paid { get; set; }
        public string product { get; set; }
        public string order_type { get; set; }
        public string vendor_code { get; set; }
        public string lmd_status { get; set; }
    }

    public class OrderList
    {
        public string Internet_no { get; set; }
        public string Order_no { get; set; }
        public string Distance_to_paid { get; set; }
        public string Total_Paid { get; set; }
        public string Product { get; set; }
        public string Order_type { get; set; }
        public string Vendor_code { get; set; }
        public string LMD_status { get; set; }
    }

    public class ModelApiSubpayment
    {
        public List<OrderList> Order_list { get; set; }
        public string Result_code { get; set; }
        public string Result_message { get; set; }
    }

    public class Response_API_Subpayment
    {
        public string Result_code { get; set; }
        public string Result_message { get; set; }
    }

}
