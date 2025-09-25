using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class ConfigurationRulePriorityView
    {
        public List<DataConfigRulePriorityTable> dataConfigRulePriority { get; set; }
    }
    public class DataConfigRulePriority
    {
        public string RULE_NAME { get; set; }
        public string PRIORITY { get; set; }
        public string LOOKUP_NAME { get; set; }
        public string LOOKUP_PARAMETER { get; set; }
        public string EFFECTIVE_DATE_START { get; set; }
        public string EFFECTIVE_DATE_END { get; set; }
        public string RULE_PARAM_ID { get; set; }
    }
    public class DataConfigRulePriorityTable
    {
        public string RULE_ID { get; set; }
        public string RULE_NAME { get; set; }
        public string PRIORITY { get; set; }
        public string LOOKUP_NAME { get; set; }
        public string LOOKUP_PARAMETER { get; set; }
        public string EFFECTIVE_DATE_START { get; set; }
        public string EFFECTIVE_DATE_END { get; set; }
        public DateTime EFFECTIVE_DATE_START_DT { get; set; }
        public DateTime EFFECTIVE_DATE_END_DT { get; set; }
        public string RULE_PARAM_ID { get; set; }
        public string LOOKUP_PARAMETER_DISPLAY { get; set; }
    }
    public class ConfigRulePriorityResponse
    {
        public ConfigRulePriorityResponse()
        {
            if (result_priority_search_cur == null)
            {
                result_priority_search_cur = new List<DataConfigRulePriorityTable>();
            }

        }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public List<DataConfigRulePriorityTable> result_priority_search_cur { get; set; }
    }

    public class DeleteConfigRulePriority
    {
        public string RULE_ID { get; set; }
    }
}
