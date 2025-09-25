using System;

namespace WBBEntity.Models
{
    public partial class FBB_VOUCHER_MASTER
    {
        public long VOUCHER_MASTER_ID { get; set; }
        public string VOUCHER_PROJECT_GROUP { get; set; }
        public string VOUCHER_PROJECT_CODE { get; set; }
        public string VOUCHER_PROJECT_DES { get; set; }
        public string PROJECT_STATUS { get; set; }
        public long TRANSACTION_ID { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    }
}
