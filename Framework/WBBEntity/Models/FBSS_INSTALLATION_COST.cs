using System;

namespace WBBEntity.Models
{
    public partial class FBSS_INSTALLATION_COST
    {
        public Nullable<decimal> ID { get; set; }
        public string SERVICE { get; set; }
        public string INS_OPTION { get; set; }
        public string VENDOR { get; set; }
        public Nullable<decimal> INTERNET { get; set; }
        public Nullable<decimal> PLAYBOX { get; set; }
        public Nullable<decimal> VOIP { get; set; }
        public string ORDER_TYPE { get; set; }
        public System.DateTime EFFECTIVE_DATE { get; set; }
        public Nullable<System.DateTime> EXPIRE_DATE { get; set; }
        public string REMARK { get; set; }
        public Nullable<decimal> LENGTH_FR { get; set; }
        public Nullable<decimal> LENGTH_TO { get; set; }
        public Nullable<decimal> OUT_DOOR_PRICE { get; set; }
        public Nullable<decimal> IN_DOOR_PRICE { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public string CREATE_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string ADDRESS_ID { get; set; }
        public Nullable<decimal> TOTAL_PRICE { get; set; }
    }
}
