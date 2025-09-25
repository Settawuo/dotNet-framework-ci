using System;

namespace WBBEntity.Models
{
    public partial class FBB_PORT_INFO
    {

        public decimal PORTID { get; set; }
        public decimal CARDID { get; set; }
        public decimal PORTNUMBER { get; set; }
        public Nullable<decimal> PORTSTATUSID { get; set; }
        public string PORTTYPE { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    }
}
