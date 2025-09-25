using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels
{
    public class CoverageAreaResultModel
    {
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

        public string PROVINCE { get; set; }
        public string AUMPHUR { get; set; }
        public decimal RESULTID { get; set; }

        public List<SPLITTER_INFO> SPLITTER_LIST { get; set; }
        public List<SPLITTER_3BB_INFO> SPLITTER_3BB_LIST { get; set; }
        public string SPLITTER_3BB_JSON { get; set; }
        public List<DSLAM_INFO> RESOURCE_LIST { get; set; }

        //20.2
        public string GRID_ID { get; set; }
        public string WTTX_MAXBANDWITH { get; set; }
        public string WTTX_COVERAGE_RESULT { get; set; }
        public List<SPLITTER_3BB_RESERVED> SPLITTER_3BB_RESERVED_LIST { get; set; }
        public bool IS_3BB_COVERAGE { get; set; }
        public string EXCLUSIVE_3BB { get; set; }
    }

    public class SPLITTER_INFO
    {
        public string Splitter_Name { get; set; }
        public decimal Distance { get; set; }
        public string Distance_Type { get; set; }
        public string Resource_Type { get; set; }
    }

    public class SPLITTER_3BB_INFO
    {
        public string Splitter_Code { get; set; }
        public string Splitter_Alias { get; set; }
        public decimal Distance { get; set; }
        public string Distance_Type { get; set; }
        public string Splitter_Port { get; set; }
        public string Splitter_Latitude { get; set; }
        public string Splitter_Longitude { get; set; }

    }

    public class DSLAM_INFO
    {
        public string Dslam_Name { get; set; }
    }

    public class SPLITTER_3BB_RESERVED
    {
        public string referenceId { get; set; }
        public string splitterCode { get; set; }
        public string splitterAlias { get; set; }
        public string distance { get; set; }
        public string splitterPort { get; set; }
        public string splitterLatitude { get; set; }
        public string splitterLongitude { get; set; }
        public string mdfName { get; set; }
        public string mdfPort { get; set; }
        public string isHome { get; set; }
    }
}
