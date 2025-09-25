using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectDormStatusQuery : IQuery<List<LovModel>>
    {
        public string Lang_Flag { get; set; }
        public bool FlagSearch { get; set; }
    }
}