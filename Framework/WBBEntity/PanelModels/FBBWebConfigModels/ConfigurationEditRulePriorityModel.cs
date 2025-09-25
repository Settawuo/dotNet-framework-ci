using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class ConfigurationEditRulePriorityView
    {
        public string ret_rule_id { get; set; }
        public string ret_rule_name { get; set; }
        public string ret_priority { get; set; }
        public string ret_lmr_flag { get; set; }
        public string ret_lookup_name { get; set; }
        public string ret_lookup_parameter { get; set; }
        public string ret_rule_param_id { get; set; }
        public string ret_effective_date_start { get; set; }
        public string ret_effective_date_end { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public List<DataConfigEDITRulePriorityTable> dataConfigEditRulePriority { get; set; }
    }

    public class DataConfigEditRulePriority
    {
        public string RULE_ID { get; set; }
    }
    public class DataConfigEDITRulePriorityTable
    {
        public string RULE_ID { get; set; }
        public string CONDITION_ID { get; set; }
        public string CONDITION_PARAMETER { get; set; }
        public string OPERATOR { get; set; }
        public string VALUE { get; set; }
    }
    public class ConfigEditRulePriorityResponse
    {
        public ConfigEditRulePriorityResponse()
        {
            if (result_priority_condition_cur == null)
            {
                result_priority_condition_cur = new List<DataConfigEDITRulePriorityTable>();
            }

        }
        public string ret_rule_id { get; set; }
        public string ret_rule_name { get; set; }
        public string ret_priority { get; set; }
        public string ret_lmr_flag { get; set; }
        public string ret_lookup_name { get; set; }
        public string ret_lookup_parameter { get; set; }
        public string ret_rule_param_id { get; set; }
        public string ret_effective_date_start { get; set; }
        public string ret_effective_date_end { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public List<DataConfigEDITRulePriorityTable> result_priority_condition_cur { get; set; }
    }
}
