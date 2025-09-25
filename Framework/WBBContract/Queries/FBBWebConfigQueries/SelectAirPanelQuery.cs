using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectAirPanelQuery : IQuery<List<AirPanelModel>>
    {
        public string region { get; set; }
        public string province { get; set; }
        public string tumbon { get; set; }
        public string aumphur { get; set; }

    }
}
