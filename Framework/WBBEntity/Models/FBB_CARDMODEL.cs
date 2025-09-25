using System;
using System.ComponentModel;

namespace WBBEntity.Models
{
    public partial class FBB_CARDMODEL
    {
        public decimal CARDMODELID { get; set; }
        [DisplayName("Model")]
        public string MODEL { get; set; }
        [DisplayName("Start Index")]
        public decimal PORTSTARTINDEX { get; set; }
        [DisplayName("Max Port")]
        public decimal MAXPORT { get; set; }
        [DisplayName("Reserve Port Spare")]
        public decimal RESERVEPORTSPARE { get; set; }
        [DisplayName("Port Type")]
        public string DATAONLY_FLAG { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        [DisplayName("Brand")]
        public string BRAND { get; set; }
    }
}
