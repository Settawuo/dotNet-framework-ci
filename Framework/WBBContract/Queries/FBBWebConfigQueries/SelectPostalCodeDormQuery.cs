using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectPostalCodeDormQuery : IQuery<List<LovModel>>
    {
        public string PROVINCE { get; set; }
        public string AUMPHUR { get; set; }
        public string TUMBON { get; set; }
        public string Lang_Flag { get; set; }
    }
}
