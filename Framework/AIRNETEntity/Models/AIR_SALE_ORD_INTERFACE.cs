using System;

namespace AIRNETEntity.Models
{
    public partial class AIR_SALE_ORD_INTERFACE
    {
        public string ORDER_NO { get; set; }
        public Nullable<DateTime> INTERFACE_DATE { get; set; }
        public string INTERFACE_BY { get; set; }
        public string INTERFACE_STATUS { get; set; }
        public string SFF_CA_NUMBER { get; set; }
        public string NON_MOBILE_NO { get; set; }
        public string MAIN_PROMOTION { get; set; }
        public string ONTOP_PROMOTION { get; set; }
        public string ERROR_REASON { get; set; }
        public string REMARK { get; set; }
        public string INTERFACE_RESULT { get; set; }
        public Nullable<DateTime> UPD_DTM { get; set; }
        public string UPD_BY { get; set; }
        public string SFF_SA_NUMBER { get; set; }
        public string SFF_BA_NUMBER { get; set; }
    }
}
