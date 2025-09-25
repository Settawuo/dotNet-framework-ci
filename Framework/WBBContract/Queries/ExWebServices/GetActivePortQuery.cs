using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class GetActivePortQuery : GetCoverageQueryBase, IQuery<SBNCheckCoverageResponse>
    {
        public int CURRENTPORTID { get; set; }
    }
}
