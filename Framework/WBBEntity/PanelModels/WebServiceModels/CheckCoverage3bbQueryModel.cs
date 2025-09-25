using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class CheckCoverage3bbQueryModel
    {
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public string coverage { get; set; }
        public List<CheckCoverage3bbSplitter> splitterList { get; set; }
        public string inServiceDate { get; set; }
    }

    public class CheckCoverage3bbSplitter
    {
        public string splitterCode { get; set; }
        public string splitterAlias { get; set; }
        public string distance { get; set; }
        public string splitterPort { get; set; }
        public string splitterLatitude { get; set; }
        public string splitterLongitude { get; set; }
    }
}
