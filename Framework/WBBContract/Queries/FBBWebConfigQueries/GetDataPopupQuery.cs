using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetDataPopupQuery : IQuery<List<ListPopupModel>>
    {
        public string p_search_column { get; set; }
        public string p_value { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public string cur { get; set; }

    }
}
