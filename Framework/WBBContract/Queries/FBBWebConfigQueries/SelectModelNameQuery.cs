using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectModelNameQuery : IQuery<List<ModelNameCardModel>>
    {
        public string DlasmBran { get; set; }
        public string CardModel { get; set; }
        public string Reserve { get; set; }
        public Decimal CardModelidID { get; set; }
        public Decimal DsalmID { get; set; }
    }
}
