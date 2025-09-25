using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.FTTx
{
    public class GetCountCoverageQuery : IQuery<CountCoverageModel>
    {

        public string Province { get; set; }
        public string Amphur { get; set; }
        public string Tumbon { get; set; }
        public string OwnerProduct { get; set; }
        public string OwnerType { get; set; }
        public string tower_th { get; set; }
        public string GROUP_AMPHUR { get; set; }
        public string Region { get; set; }
    }
}
