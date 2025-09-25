using System.Collections.Generic;
using WBBEntity.PanelModels.ShareplexModels;

namespace WBBContract.Queries.FBBShareplex
{
    public class GetLowUtilizeSaleReportQuery : IQuery<List<LowUtilizeSaleReportList>>
    {
        public string ret_code { get; set; }
        public string ret_message { get; set; }

        public string p_location_code { get; set; }
    }
}
