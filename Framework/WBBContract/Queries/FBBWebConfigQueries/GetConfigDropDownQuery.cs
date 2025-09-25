using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetConfigDropDownQuery : IQuery<List<GetConfigDropDownModel>>
    {
        public string config_name { get; set; }
        public string symptom_group { get; set; }
        public string province_th { get; set; }
        public string district_th { get; set; }
    }
}
