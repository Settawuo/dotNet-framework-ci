using System;

namespace WBBEntity.Models
{
    public partial class FBB_ZIPCODE
    {
        public decimal ZIPCODE_ID { get; set; }
        public string ZIPCODE_ROWID { get; set; }
        public string ZIPCODE { get; set; }
        public string LANG_FLAG { get; set; }
        public string TUMBON { get; set; }
        public string AMPHUR { get; set; }
        public string PROVINCE { get; set; }
        public string COUNTRY { get; set; }
        public string STATUS { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string REGION_CODE { get; set; }
        public string GROUP_AMPHUR { get; set; }
        public string SUB_REGION { get; set; }
    }
}
