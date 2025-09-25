using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class GetAvaliablePortQuery : GetCoverageQueryBase, IQuery<SBNCheckCoverageResponse>
    {
        public int CURRENTPORTID { get; set; }
    }
}
