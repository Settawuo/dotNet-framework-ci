using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class CheckStatusOnOffQuery : IQuery<ConfigurationOnOffServices>
    {
        public string p_dormitory_name { get; set; }
        public string p_building { get; set; }
        public decimal p_return_code { get; set; }

    }
}
