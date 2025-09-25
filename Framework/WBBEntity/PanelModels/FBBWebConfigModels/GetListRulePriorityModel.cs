using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class GetListRulePriorityModel
    {
        public string RULE_NAME { get; set; }
        public string PRIORITY { get; set; }
        public string LOOKUP_NAME { get; set; }
        public string LOOKUP_PARAMETER { get; set; }
        public string EFFECTIVE_DATE_START { get; set; }
        public string EFFECTIVE_DATE_END { get; set; }
        public string RULE_PARAM_ID { get; set; }
    }
}
