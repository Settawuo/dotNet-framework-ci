using System;
using System.ComponentModel;

namespace WBBEntity.Models
{
    public class FBB_DSLAMMODEL
    {
        public decimal DSLAMMODELID { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        [DisplayName("Start Index")]
        public decimal SLOTSTARTINDEX { get; set; }
        [DisplayName("Model")]
        public string MODEL { get; set; }
        [DisplayName("Brand")]
        public string BRAND { get; set; }
        [DisplayName("Max Slot")]
        public decimal MAXSLOT { get; set; }
        public string ACTIVEFLAG { get; set; }
        [DisplayName("Brand-คำย่อ")]
        public string SH_BRAND { get; set; }
    }
}
