using System;

namespace WBBEntity.Models
{
    public partial class FBB_PROGRAM
    {
        public decimal PROGRAM_ID { get; set; }
        public string PROGRAM_CODE { get; set; }
        public string PROGRAM_NAME { get; set; }
        public string PROGRAM_DESCRIPTION { get; set; }
        public string ACTIVE_FLG { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public Nullable<decimal> PARENT_ID { get; set; }
        public Nullable<decimal> ORDER_BY { get; set; }
    }
}
