using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetPAYGTransAirnetParameterQuery : IQuery<PAYGTransAirnetParameterListResult>
    {
        public string test { get; set; }
    }
}
