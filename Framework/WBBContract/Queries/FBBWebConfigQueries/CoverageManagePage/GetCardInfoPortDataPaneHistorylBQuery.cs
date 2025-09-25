using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class
        GetCardInfoPortDataPaneHistorylBQuery : IQuery<List<portPanelHittory>>
    {
        public decimal DSLAMID { get; set; }
        public decimal CardID { get; set; }
        public decimal PorID { get; set; }

    }
}
