using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class DataCoverageAreaResultModel
    {       
        public decimal? RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public string ADDRRESS_TYPE { get; set; }
        public string BUILDING_NAME { get; set; }
        public string BUILDING_NO { get; set; }
        public string FLOOR_NO { get; set; }
        public string ADDRESS_NO { get; set; }
        public decimal? MOO { get; set; }
        public string SOI { get; set; }
        public string ROAD { get; set; }
        public string POSTAL_CODE { get; set; }
        public string ZIPCODE_ROWID { get; set; }
        public string LANGUAGE { get; set; }
        public string TUMBON { get; set; }
        public string AMPHUR { get; set; }
        public string PROVINCE { get; set; }
        public string LANG_FLAG { get; set; }
        public string CONTACTNUMBER { get; set; }

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
