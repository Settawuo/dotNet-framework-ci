using System;

namespace WBBEntity.Models
{
    public partial class FBB_VOUCHER_PIN
    {
        public long VOUCHER_PIN_ID { get; set; }
        public string VOUCHER_PIN { get; set; }
        public string PIN_STATUS { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> EXPIRE_DATE { get; set; }
        public long TRANSACTION_ID { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public long VOUCHER_MASTER_ID { get; set; }
        public long LOT { get; set; }
    }
}
