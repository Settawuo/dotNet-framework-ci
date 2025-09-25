using System;

namespace WBBEntity.Models
{
    public partial class FBSS_FIXED_ASSET_CONFIG
    {
        public string PROGRAM_CODE { get; set; }
        public string PROGRAM_NAME { get; set; }
        public string COM_CODE { get; set; }
        public string ASSET_CLASS_GI { get; set; }
        public string ASSET_CLASS_INS { get; set; }
        public string COST_CENTER { get; set; }
        public decimal? QUANTITY { get; set; }
        public string MOVEMENT_TYPE_OUT { get; set; }
        public string XREF1_HD { get; set; }
        public string EVA4_GI { get; set; }
        public string EVA4_INS { get; set; }
        public string DOCUMENT_TYPE { get; set; }
        public string CURRENCY { get; set; }
        public decimal? RATE { get; set; }
        public string ACCOUNT { get; set; }
        public System.DateTime? CREATE_DATETIME { get; set; }
        public System.DateTime? MODIFY_DATETIME { get; set; }
        public string MOVEMENT_TYPE_IN { get; set; }
        public Nullable<System.DateTime> EFFECTIVE_DATE { get; set; }
        public Nullable<System.DateTime> EXPIRE_DATE { get; set; }
    }
}
