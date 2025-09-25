using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.FBBShareplex
{
    public class GetLovShareplexQuery : IQuery<List<LovValueModel>>
    {
        public string LovName { get; set; }
        public string LovType { get; set; }
    }
}
