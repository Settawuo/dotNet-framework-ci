using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetDataAddressBillModel
    {
        public string return_code { get; set; }
        public string return_message { get; set; }
        public List<BillAddressList> address_curror { get; set; }

    }

    public class BillAddressList
    {
        public string Address_bill { get; set; }


    }

}
