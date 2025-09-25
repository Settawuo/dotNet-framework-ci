using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelecProvinceAirQuery : IQuery<List<DropdownModel>>
    {
        public string REGION_CODE { get; set; }
    }
}
