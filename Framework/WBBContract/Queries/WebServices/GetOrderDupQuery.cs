using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class GetOrderDupQuery : IQuery<List<OrderDupModel>>
    {
        public string p_id_card { get; set; }

        public string p_eng_flag { get; set; }

    }
}
