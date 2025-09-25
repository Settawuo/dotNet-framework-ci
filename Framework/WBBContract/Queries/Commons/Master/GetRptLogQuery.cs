using System.Collections.Generic;
using WBBEntity.Models;

namespace WBBContract.Queries.Commons.Masters
{
    public class GetRptLogQuery : IQuery<List<FBB_RPT_LOG>>
    {
        public string UserName { get; set; }

    }
}
