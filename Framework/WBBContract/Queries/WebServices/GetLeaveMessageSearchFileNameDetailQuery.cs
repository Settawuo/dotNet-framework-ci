using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class GetLeaveMessageSearchFileNameDetailQuery : IQuery<List<SearchLeaveMsgFileNameDetailList>>
    {
        public string P_FILE_NAME { get; set; }
        public string P_USERNAME { get; set; }
        public string P_STATUS { get; set; }

        // return code
        public int return_code { get; set; }
        public string return_message { get; set; }
    }
}
