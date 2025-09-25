using System;

namespace WBBEntity.Models
{
    public partial class FBB_COVERAGEAREA_RESULT
    {
        public decimal RESULTID { get; set; }
        public decimal CVRID { get; set; }
        public string NODENAME { get; set; }
        public string TOWER { get; set; }
        public string PREFIXNAME { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string CONTACTNUMBER { get; set; }
        public Nullable<decimal> FLOOR { get; set; }
        public string ISONLINENUMBER { get; set; }
        public string ADDRESS_NO { get; set; }
        public decimal MOO { get; set; }
        public string SOI { get; set; }
        public string ROAD { get; set; }
        public string COVERAGETYPE { get; set; }
        public string COVERAGERESULT { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string PRODUCTTYPE { get; set; }
        public string ZIPCODE_ROWID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        //public string SELECTPRODUCT { get; set; }

        public Nullable<decimal> RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public string RETURN_ORDER { get; set; }
        public string OWNER_PRODUCT { get; set; }

        public string TRANSACTION_ID { get; set; }
    }
}
