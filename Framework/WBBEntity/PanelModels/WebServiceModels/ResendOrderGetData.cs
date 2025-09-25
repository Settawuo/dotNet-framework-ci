using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class ResendOrderGetData
    {
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public string return_transactions { get; set; }
        public List<string> item_json { get; set; }
    }

}
