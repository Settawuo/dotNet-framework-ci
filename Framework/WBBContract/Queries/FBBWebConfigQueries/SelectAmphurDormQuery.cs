using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;


namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectAmphurDormQuery : IQuery<List<LovModel>>
    {
        public string PROVINCE { get; set; }
        public string Lang_Flag { get; set; }
    }
}
