using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class GetAirnetWirelessCoverageQuery : GetCoverageQueryBase, IQuery<SBNCheckCoverageResponse>
    {
        public string LAT { get; set; }
        public string LNG { get; set; }
        public string COVERAGETYPE { get; set; }
        public decimal FLOOR { get; set; }
        public string SSO { get; set; }
    }
}
