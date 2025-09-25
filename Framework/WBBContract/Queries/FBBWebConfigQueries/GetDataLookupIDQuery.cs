using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetDataLookupIDQuery : IQuery<List<GetListDataLookupIDModel>>
    {
        public string LOOKUP_NAME { get; set; }
        public int PAGE_INDEX { get; set; }
        public int PAGE_SIZE { get; set; }
    }
}
