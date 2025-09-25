using System;

namespace WBBEntity.Models
{
    public partial class FBB_FBSS_COVERAGEAREA_RESULT
    {
        public decimal RESULTID { get; set; }
        public string ADDRRESS_TYPE { get; set; }
        public string POSTAL_CODE { get; set; }
        public string SUB_DISTRICT_NAME { get; set; }
        public string LANGUAGE { get; set; }
        public string BUILDING_NAME { get; set; }
        public string BUILDING_NO { get; set; }
        public string PHONE_FLAG { get; set; }
        public string FLOOR_NO { get; set; }
        public string ADDRESS_NO { get; set; }
        public decimal? MOO { get; set; }
        public string SOI { get; set; }
        public string ROAD { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string UNIT_NO { get; set; }
        public string COVERAGE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string ACCESS_MODE_LIST { get; set; }
        public string PLANNING_SITE_LIST { get; set; }
        public string IS_PARTNER { get; set; }
        public string PARTNER_NAME { get; set; }
        public string PREFIXNAME { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string CONTACTNUMBER { get; set; }
        public string PRODUCTTYPE { get; set; }
        public string ZIPCODE_ROWID { get; set; }
        public decimal? RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public string RETURN_ORDER { get; set; }
        public string OWNER_PRODUCT { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string CONTACT_EMAIL { get; set; }
        public string CONTACT_LINE_ID { get; set; }
        public string LOCATION_CODE { get; set; }
        public string ASC_CODE { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string SALE_NAME { get; set; }
        public string LOCATION_NAME { get; set; }
        public string SUB_REGION { get; set; }
        public string REGION_NAME { get; set; }
        public string ASC_NAME { get; set; }
        public string CHANNEL_NAME { get; set; }
        public string SALE_CHANNEL { get; set; }
        //R21.2
        public string ADDRESS_TYPE_DTL { get; set; }
        public string REMARK { get; set; }
        public string TECHNOLOGY { get; set; }
        public string PROJECTNAME { get; set; }

        // onservice special
        public string COVERAGE_AREA { get; set; }
        public string COVERAGE_STATUS { get; set; }
        public string COVERAGE_SUBSTATUS { get; set; }
        public string COVERAGE_CONTACTEMAIL { get; set; }
        public string COVERAGE_CONTACTTEL { get; set; }
        public string COVERAGE_GROUPOWNER { get; set; }
        public string COVERAGE_CONTACTNAME { get; set; }
        public string COVERAGE_NETWORKPROVIDER { get; set; }
        public string COVERAGE_FTTHDISPLAYMESSAGE { get; set; }
        public string COVERAGE_WTTXDISPLAYMESSAGE { get; set; }
    }
}
