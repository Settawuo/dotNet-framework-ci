using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class SaveConfigRulePriorityCommand
    {
        public string Rule_id { get; set; }
        public string Rule_name { get; set; }
        public string Priority { get; set; }
        public string Lookup_name { get; set; }
        public List<Lookup_param_list> Lookup_param_list { get; set; }
        public string effective_start { get; set; }
        public string effective_end { get; set; }
        public string Lmr_flag { get; set; }
        public List<Condition_list> Condition_list { get; set; }
        public string Create_by { get; set; }
        //public string Modified_by { get; set; }

        // for return
        public string return_code { get; set; }
        public string return_msg { get; set; }
    }
    public class Lookup_param_list
    {
        public string Param_rule_id { get; set; }
        public string Param_name { get; set; }
        public string Param_flag { get; set; }
    }
    public class Condition_list
    {
        public string Condition_id { get; set; }
        public string Condition_parameter { get; set; }
        public string Conditaion_operator { get; set; }
        public string Conditaion_value { get; set; }
        public string Condition_flag { get; set; }

    }
}
