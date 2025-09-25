using System;

namespace WBBEntity.Models
{
    public partial class FBB_COVERAGEAREA_RELATION
    {




        public decimal CVRRELATIONID { get; set; }
        public decimal CVRID { get; set; }
        public decimal DSLAMID { get; set; }
        public string TOWERNAME_EN { get; set; }
        public string TOWERNAME_TH { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }

        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }



    }
}
