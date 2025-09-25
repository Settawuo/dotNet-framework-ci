using System.Collections.Generic;
using WBBEntity.Models;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectHistoryLogQuery : IQuery<List<FBB_HISTORY_LOG>>
    {
        public string Application { get; set; }
        public string Ref_Name { get; set; }
        public string Ref_Key { get; set; }
        public bool FirstLoad { get; set; }
    }
}
