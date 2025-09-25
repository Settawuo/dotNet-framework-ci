using System;

namespace WBBEntity.Models
{
    public partial class FBB_MESSAGE_LOG
    {
        public decimal CUST_ROW_ID { get; set; }
        public string PROCESS_NAME { get; set; }
        public string CREATE_USER { get; set; }
        public decimal RETURN_CODE { get; set; }
        public string RETURN_DESC { get; set; }
        public string REMARK { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
    }
}
