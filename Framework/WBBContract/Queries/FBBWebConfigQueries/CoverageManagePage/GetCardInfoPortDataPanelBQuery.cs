using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class
        GetCardInfoPortDataPanelBQuery : IQuery<List<CoveragePortPanel>>
    {
        public decimal DSLAMID { get; set; }
        public decimal CardID { get; set; }
        public string Dispaly { get; set; }

    }
}
