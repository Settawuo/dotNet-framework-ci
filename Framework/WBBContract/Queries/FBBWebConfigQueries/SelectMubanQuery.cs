using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectMubanQuery : IQuery<List<LovModel>>
    {
        public string province { get; set; }
        public string aumphur { get; set; }
        public string tumbon { get; set; }
        public string Language { get; set; }
        public string SSO { get; set; }
    }
}
