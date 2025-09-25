using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectTumbonQuery : IQuery<List<LovModel>>
    {
        public string REGION_CODE { get; set; }
        public string PROVINCE { get; set; }
        public string AUMPHUR { get; set; }
        public string Lang_Flag { get; set; }
    }
}
