using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage
{
    public class GetAWCAllQuery : IQuery<List<AWCexportResultlist>>
    {
        public string region { get; set; }
        public string province { get; set; }
        public string aumphur { get; set; }
        public string tumbon { get; set; }
        public string APname { get; set; }
    }
}
