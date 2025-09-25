using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class QueryOrderModel
    {
        public string RESULT { get; set; }
        public string RESULT_DESC { get; set; }
        public List<Order_Details> Order_Details_List { get; set; }
    }

    public class Order_Details
    {
        public string FIBRENET_ID { get; set; }
        public string ORDER_TYPE { get; set; }
        public string TRANSACTION_NUMBER { get; set; }
        public string TRANSACTION_STATE { get; set; }
        public string TRANSACTION_DATE { get; set; }
        public string APPOINTMENT_DATE { get; set; }
        public string APPOINTMENT_TIMESLOT { get; set; }
        public string TRANACTION_NOTE { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string SERVICE_PACKAGE_NAME { get; set; }
        public string EVENT { get; set; }
        public List<Activity_Details> ACTIVITY_DETAILS { get; set; }
    }

    public class Activity_Details
    {
        public string WORK_ORDER_NO { get; set; }
        public string SUBJECT { get; set; }
        public string WORK_ORDER_STATE { get; set; }
        public string ACTIVITY { get; set; }
        public string CREATED_DATE { get; set; }
        public string WARNING_DATE { get; set; }
        public string TIMEOUT_DATE { get; set; }
        public string COMPLETED_DATE { get; set; }
        public string REMARKS { get; set; }
        public string HANDLE_ORG { get; set; }
        public string HANDLE_JOB { get; set; }
        public string HANDLE_STAFF { get; set; }
        public string APPOINTMENT_DATE { get; set; }
        public string APPOINTMENT_TIMESLOT { get; set; }
        public string FOA_REJECT_REASON { get; set; }
        public string CURRENT_WORK_ORDER_FLAG { get; set; }
    }

    public class ReleaseTimeSlotModel
    {
        public string RESULT { get; set; }
        public string RESULT_DESC { get; set; }
    }

    public class ResReleaseModel
    {
        public string RESULT { get; set; }
        public string RESULT_DESC { get; set; }
    }
}
