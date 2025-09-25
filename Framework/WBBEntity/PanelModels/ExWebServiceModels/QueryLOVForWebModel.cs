using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class QueryLOVForWebModel
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<QueryLOVForWebData> LIST_LOV_CUR { get; set; }
    }

    public class QueryLOVForWebData
    {
        public string LOV_TYPE { get; set; }
        public string LOV_NAME { get; set; }
        public string DISPLAY_VAL { get; set; }
        public string LOV_VAL1 { get; set; }
        public string LOV_VAL2 { get; set; }
        public string LOV_VAL3 { get; set; }
        public string LOV_VAL4 { get; set; }
        public string LOV_VAL5 { get; set; }
        public string ACTIVEFLAG { get; set; }
        public decimal? ORDER_BY { get; set; }
        public string DEFAULT_VALUE { get; set; }
    }
}
