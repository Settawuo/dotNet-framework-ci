using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetListLookupParameterQuery : IQuery<ConfigurationLookupParamView>
    {
        public string p_lookup_name { get; set; }
    }
}
