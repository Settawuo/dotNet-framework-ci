using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    class GetRulePriorityTableQuery : IQuery<ConfigurationRulePriorityView>
    {
        public string RULE_NAME { get; set; }
        public string PRIORITY { get; set; }
        public string LOOKUP_NAME { get; set; }
        public string EFTDATE_FROM { get; set; }
        public string EFTDATE_TO { get; set; }
    }
}
