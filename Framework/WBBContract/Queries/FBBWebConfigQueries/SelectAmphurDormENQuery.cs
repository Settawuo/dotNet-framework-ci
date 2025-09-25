using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectAmphurDormENQuery : IQuery<List<LovModel>>
    {
        public string PROVINCE { get; set; }
        public string Lang_Flag { get; set; }
    }
}