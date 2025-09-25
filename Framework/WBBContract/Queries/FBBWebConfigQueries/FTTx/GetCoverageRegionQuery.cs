using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.FTTx
{
    public class GetCoverageRegionQuery : IQuery<List<GridFTTxModel>>
    {
        public string Region { get; set; }
        public string Province { get; set; }
        public string Amphur { get; set; }
        public string Tumbon { get; set; }
        public string OwnerProduct { get; set; }
        public string OwnerType { get; set; }
        public string tower_th { get; set; }
        public string tower_en { get; set; }
        public string FlagDataGroupAmper { get; set; }
        public string Service_Type { get; set; }
    }
}
