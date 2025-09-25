using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class DeleteConfigurationRulePriorityCommand
    {
        public string RULE_ID { get; set; }
        public string return_code { get; set; }
        public string return_msg { get; set; }
    }
}
