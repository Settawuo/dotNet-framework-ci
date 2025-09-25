using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetCoverageAreaByCVRQuery : IQuery<CoverageAreaModel>
    {
        public decimal CVRID { get; set; }
    }
}
