using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetCardInfoPortPanelDataQuery : IQuery<List<CoveragePortPanelGrid>>
    {
        public decimal DSLAMID { get; set; }
        public decimal CardID { get; set; }
        public string ResultReaderdataPor { get; set; }
        public string NodeID { get; set; }
        public decimal MaxCard_Info { get; set; }
    }
}
