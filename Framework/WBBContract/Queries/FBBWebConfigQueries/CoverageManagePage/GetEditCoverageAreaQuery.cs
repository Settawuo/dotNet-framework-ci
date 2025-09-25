using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetEditCoverageAreaQuery : IQuery<CoverageSitePanelModel>
    {
        public decimal? ContactId { get; set; }
        public bool GetForEdit { get; set; }
    }
}
