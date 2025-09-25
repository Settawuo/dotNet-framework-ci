using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class AutoCheckCoverageModel
    {
        public AutoCheckCoverageModel()
        {
            if (NO_COVERAGE_RESULT == null)
                NO_COVERAGE_RESULT = new List<NoCoverageModel>();
        }
        public string RET_CODE { get; set; }
        public string RET_MESSAGE { get; set; }
        public List<NoCoverageModel> NO_COVERAGE_RESULT { get; set; }
    }

    public class NoCoverageModel
    {
        public string RESULT_ID { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
    }
}
