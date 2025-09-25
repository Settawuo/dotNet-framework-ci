using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Master
{
    public class GetCoverageAreaQuery : IQuery<List<CoverageValueModel>>
    {
        public int CurrentCulture { get; set; }
        public string Province { get; set; }
        public string Amphur { get; set; }
        public string ZipCodeId { get; set; }
        public string SSO { get; set; }
    }
}
