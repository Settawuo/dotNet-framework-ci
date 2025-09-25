using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class ArchiveInterfaceLogQuery : IQuery<List<ArchiveInterfaceLogSendMailDetailList>>
    {
        public decimal p_date { get; set; }
        public string p_type { get; set; }

        //return value
        public int ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
