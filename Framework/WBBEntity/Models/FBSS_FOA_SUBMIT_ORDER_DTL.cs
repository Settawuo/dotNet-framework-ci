using System;

namespace WBBEntity.Models
{
    public partial class FBSS_FOA_SUBMIT_ORDER_DTL
    {
        public string ORDER_NO { get; set; }
        public string SN { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string SNPATTERN { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public Nullable<System.DateTime> CREATE_DATETIME { get; set; }
        public Nullable<System.DateTime> MODIFY_DATETIME { get; set; }
        public string STATUS { get; set; }
        public string SERVICE_NAME { get; set; }
        public decimal? SERVICE_SEQ { get; set; }
    }
}
