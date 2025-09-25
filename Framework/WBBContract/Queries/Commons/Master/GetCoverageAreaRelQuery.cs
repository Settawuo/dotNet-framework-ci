using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Master
{
    public class GetCoverageAreaRelQuery : IQuery<List<CoverageRelValueModel>>
    {
        public int CurrentCulture { get; set; }
        public string NodeName { get; set; }
    }
}
