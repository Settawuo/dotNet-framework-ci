using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class TrackingCompetitorRptQuery : IQuery<List<TrackingCompetitorRptList>>
    {
        public string order_date_from { get; set; }
        public string order_date_to { get; set; }
        public string order_status { get; set; }

        //return
        public int ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
