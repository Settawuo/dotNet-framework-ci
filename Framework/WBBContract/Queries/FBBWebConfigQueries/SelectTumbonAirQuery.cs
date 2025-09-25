using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectTumbonAirQuery : IQuery<List<DropdownModel>>
    {
        public string REGION_CODE { get; set; }
        public string PROVINCE { get; set; }
        public string AUMPHUR { get; set; }
    }
}
