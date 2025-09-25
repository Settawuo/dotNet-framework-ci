using System;

namespace WBBEntity.Models
{
    public partial class FBB_CFG_LOV
    {
        public decimal LOV_ID { get; set; }
        public Nullable<decimal> PAR_LOV_ID { get; set; }
        public string LOV_TYPE { get; set; }
        public string LOV_NAME { get; set; }
        public string DISPLAY_VAL { get; set; }
        public string LOV_VAL1 { get; set; }
        public string LOV_VAL2 { get; set; }
        public string LOV_VAL3 { get; set; }
        public string LOV_VAL4 { get; set; }
        public string LOV_VAL5 { get; set; }
        public string ACTIVEFLAG { get; set; }
        public Nullable<decimal> ORDER_BY { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string DEFAULT_VALUE { get; set; }
        public byte[] IMAGE_BLOB { get; set; }
    }
}
