using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GoodsMovementKAFKAQuery : IQuery<GoodsMovementKAFKAQueryModel>
    {
        public string action { get; set; }

        //model for producer
        public string json_producer { get; set; }
        public string in_foa_producer { get; set; }
        public List<string> item_json { get; set; }


        //return value
        public string Return_Code { get; set; }
        public string Return_Message { get; set; }
        public string Return_Transactions { get; set; }
    }
}
