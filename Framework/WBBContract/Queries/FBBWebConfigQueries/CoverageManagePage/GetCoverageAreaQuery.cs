using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetCoverageAreaQuery : IQuery<CoverageSitePanelModel>
    {
        public string RegionCode { get; set; }
        public string Province { get; set; }
        public string Amphur { get; set; }
        public string NodeStaus { get; set; }
        public string NodeName { get; set; }
        public string IpRanCode { get; set; }
        public string LocationCode { get; set; }
        public string Port { get; set; }
        public string zipcode { get; set; }
    }
}
