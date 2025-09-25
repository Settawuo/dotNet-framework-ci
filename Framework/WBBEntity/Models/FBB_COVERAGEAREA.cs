using System;

namespace WBBEntity.Models
{
    public partial class FBB_COVERAGEAREA
    {
        public decimal CVRID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public System.DateTime? UPDATED_DATE { get; set; }
        public string LOCATIONCODE { get; set; }
        public string BUILDINGCODE { get; set; }
        public string NODENAME_EN { get; set; }
        public string NODENAME_TH { get; set; }
        public string NODETYPE { get; set; }
        public string NODESTATUS { get; set; }
        public string ACTIVEFLAG { get; set; }
        public decimal? MOO { get; set; }
        public string SOI_TH { get; set; }
        public string ROAD_TH { get; set; }
        public string SOI_EN { get; set; }
        public string ROAD_EN { get; set; }
        public string ZIPCODE { get; set; }
        public string IPRAN_CODE { get; set; }
        public string CONTACT_NUMBER { get; set; }
        public string FAX_NUMBER { get; set; }
        public string REGION_CODE { get; set; }
        public decimal? CONTACT_ID { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string COMPLETE_FLAG { get; set; }
        public string TIE_FLAG { get; set; }
        public DateTime? ONTARGET_DATE_EX { get; set; }
        public DateTime? ONTARGET_DATE_IN { get; set; }
    }
}
