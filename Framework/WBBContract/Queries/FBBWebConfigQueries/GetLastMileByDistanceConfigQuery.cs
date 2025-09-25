using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetLastMileByDistanceConfigQuery : IQuery<List<LovModel>>
    {
        public string LOV_TYPE { get; set; }
        public string LOV_VAL5 { get; set; }
    }
}
