using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    //public class GetListEditRulePriorityQuery
    public class GetListEditRulePriorityQuery : IQuery<ConfigurationEditRulePriorityView>
    {
        public string RULE_ID { get; set; }
    }
}
