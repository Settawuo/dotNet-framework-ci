using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{

    public class GetCoverageRegionQuery : GetCoverageQueryBase, IQuery<SBNCheckCoverageResponse>
    {
        public string Province { get; set; }
        public string Aumphur { get; set; }
        public string Tambon { get; set; }
        public string Language { get; set; }
        public string SSO { get; set; }
        public string ServiceType { get; set; }
    }
}
