using System;

namespace WBBEntity.Models
{
    public class FBB_DSLAM_INFO
    {



        public decimal DSLAMID { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public decimal DSLAMNUMBER { get; set; }
        public decimal DSLAMMODELID { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string NODEID { get; set; }
        public string REGION_CODE { get; set; }
        public string LOT_NUMBER { get; set; }
        public decimal? CVRID { get; set; }
    }
}
