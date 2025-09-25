using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetLovConfigLMRQuery : IQuery<List<ConfigurationLMREmailModel>>
    {
        public string LovType { get; set; }
        public string LovValue5 { get; set; }

    }
}
