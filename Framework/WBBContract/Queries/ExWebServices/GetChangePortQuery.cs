using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class GetChangePortQuery : GetCoverageQueryBase, IQuery<SBNCheckCoverageResponse>
    {
        public int CURRENTPORTID { get; set; }
        public string CHANGEPORT_REASON { get; set; }
    }
}
