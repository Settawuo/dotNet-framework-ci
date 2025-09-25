using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetListRulePriorityQuery : IQuery<ConfigurationRulePriorityView>
    {
        public string RULE_ID { get; set; }
        public string RULE_NAME { get; set; }
        public decimal PRIORITY { get; set; }
        public string LOOKUP_NAME { get; set; }
        public string EFFECTIVE_DATE_START { get; set; }
        public string EFFECTIVE_DATE_END { get; set; }
    }
}
