using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{


    public class SelectAmphurFttxQuery : IQuery<List<DropdownModel>>
    {
        public string REGION_CODE { get; set; }
        public string PROVINCE { get; set; }
    }
}
