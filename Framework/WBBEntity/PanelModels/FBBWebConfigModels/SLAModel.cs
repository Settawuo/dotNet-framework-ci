using System;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class AppointmentDetailModel
    {
        public string customer_order_no { get; set; }
        public string access_number { get; set; }
        public string cust_name { get; set; }
        public string WORK_ORD_NO { get; set; }
        public string JOB_TYPE { get; set; }
        public string SERVICE { get; set; }
        public Nullable<DateTime> ORDER_CREATE_DATE { get; set; }
        public Nullable<DateTime> CREATED_PROSPECT_DATE { get; set; }
        public Nullable<DateTime> ALREADY_APPOINTMENT_DATE { get; set; }
        public string SLA { get; set; }
    }

    public class AppointmentSummaryModel
    {
        public Nullable<DateTime> ORDER_CREATE_DATE { get; set; }
        public string SERVICE { get; set; }
        public decimal SixHR { get; set; }
        public decimal TWHR { get; set; }
        public decimal ETHR { get; set; }
        public decimal TFHR { get; set; }
        public decimal FEHR { get; set; }
        public decimal OverHR { get; set; }
        public decimal ONPROCESS { get; set; }
        public decimal TOTAL { get; set; }
    }

}
