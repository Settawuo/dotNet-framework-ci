using System;
namespace WBBEntity.Models
{
    public partial class FBB_SPECIALIST
    {
        public decimal ID { get; set; }
        public string CHANNEL_SALES_CODE { get; set; }
        public string CHANNEL_SALES_NAME { get; set; }
        public decimal IS_STAFF { get; set; }
        public decimal IS_PARTNER { get; set; }
        public string REMARK { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string CREATE_BY { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public string UPDATE_BY { get; set; }
    }
}
