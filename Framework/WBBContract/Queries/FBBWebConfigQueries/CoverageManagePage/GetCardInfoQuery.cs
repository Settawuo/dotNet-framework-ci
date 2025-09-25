using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{
    public class GetCardInfoQuery : IQuery<List<CardPanel>>
    {
        public decimal DSLAMID { get; set; }
    }
}
