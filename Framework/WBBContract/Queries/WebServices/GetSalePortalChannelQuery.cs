using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetSalePortalChannelQuery : IQuery<List<SalePortalChannelModel>>
    {
        //return
        public int ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
