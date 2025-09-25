using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class GetReAvailableQuery : GetCoverageQueryBase, IQuery<SBNCheckCoverageResponse>
    {
        public int CURRENTPORTID { get; set; }
    }
}
