using System;

namespace WBBEntity.Models
{
    public class FBSS_FIXED_ASSET_SN_ACT
    {
        public string ACCESS_NUMBER { get; set; }
        public string SN { get; set; }
        public string COMPANY_CODE { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public decimal? SERVICE_SEQ { get; set; }
        public string SNPATTERN { get; set; }
        public string SN_STATUS { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string ASSET_CODE { get; set; }
        public string MATERIAL_DOC { get; set; }
        public string DOC_YEAR { get; set; }
        public string MATERIAL_DOC_MT { get; set; }
        public string DOC_YEAR_MT { get; set; }
        public string MATERIAL_DOC_RET { get; set; }
        public string DOC_YEAR_RET { get; set; }
    }
}
