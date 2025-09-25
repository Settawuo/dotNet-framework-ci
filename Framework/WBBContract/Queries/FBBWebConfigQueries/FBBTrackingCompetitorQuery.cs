using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class FBBTrackingCompetitorQuery : IQuery<List<FBBTrackingCompetitorModel>>
    {
        public int ret_code { get; set; }
        public string ret_msg { get; set; }
    }

    public class GetFBBTrackingCompetitorQuery : IQuery<List<FBBTrackingCompetitorModel>>
    {
        public string ErrorMessage { get; set; }

        public int ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
