using System.Collections.Generic;
using WBBEntity.PanelModels.ShareplexModels;

namespace WBBContract.Queries.FBBHVR
{
    public class GetLowUtilizeSaleReportHVRQuery : IQuery<List<LowUtilizeSaleReportList>>
    {
        public string ret_code { get; set; }
        public string ret_message { get; set; }

        public string p_location_code { get; set; }
    }
}
