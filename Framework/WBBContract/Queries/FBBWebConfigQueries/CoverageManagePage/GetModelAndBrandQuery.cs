using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetModelAndBrandQuery : IQuery<DropdownModel>
    {
        public string RegionCode { get; set; }
        public string LotNo { get; set; }
    }
}
