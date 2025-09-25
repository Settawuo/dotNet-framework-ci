using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage
{
    public class GetDupfileQuery : IQuery<Dupfile>
    {
        public string file_name { get; set; }
        public string user { get; set; }
    }
}
