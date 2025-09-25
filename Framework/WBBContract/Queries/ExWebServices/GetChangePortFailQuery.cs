using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class GetChangePortFailQuery : GetCoverageQueryBase, IQuery<SBNCheckCoverageResponse>
    {
        public int CURRENTPORTID { get; set; }
    }
}
