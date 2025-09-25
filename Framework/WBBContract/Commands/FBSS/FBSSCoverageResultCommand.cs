using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Commands.FBSS
{
    public class FBSSCoverageResultCommand : CommandBase
    {
        public FBSSCoverageResultCommand()
        {
            this.RETURN_CODE = -1;
        }

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
        public decimal MOO { get; set; }
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
        public string USERNAME { get; set; }
        public string EMAIL { get; set; }
        public string LINEID { get; set; }
        public string LOCATION_CODE { get; set; }
        public string ASC_CODE { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string SALE_FIRSTNAME { get; set; }
        public string SALE_LASTNAME { get; set; }
        public string LOCATION_NAME { get; set; }
        public string SUB_REGION { get; set; }
        public string REGION { get; set; }
        public string ASC_NAME { get; set; }
        public string CHANNEL_NAME { get; set; }
        public string SALE_CHANNEL { get; set; }
        //R21.2
        public string ADDRESS_TYPE_DTL { get; set; }
        public string REMARK { get; set; }
        public string TECHNOLOGY { get; set; }
        public string PROJECTNAME { get; set; }

        // onservice special
        public string COVERAGEAREA { get; set; }
        public string STATUS { get; set; }
        public string SUBSTATUS { get; set; }
        public string CONTACTEMAIL { get; set; }
        public string CONTACTTEL  { get; set; }
        public string GROUPOWNER  { get; set; }
        public string CONTACTNAME  { get; set; }
        public string NETWORKPROVIDER  { get; set; }
        public string FTTHDISPLAYMESSAGE  { get; set; }
        public string WTTXDISPLAYMESSAGE { get; set; }

        public FBSSCoverageResult FBSSCoverageResult { get; set; }
    }

    public class CoverageAreaLogCommand
    {
        public string Return_Code { get; set; }
        public string Return_Message { get; set; }
    }
}
