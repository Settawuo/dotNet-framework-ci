using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class SalePortalLeaveMessageQuery : IQuery<List<SalePortalLeaveMessageList>>
    {
        public string p_reg_date_from { get; set; }
        public string p_reg_date_to { get; set; }
        public string p_refference_no { get; set; }
        public string p_customer_name { get; set; }
        public string p_contact_mobile { get; set; }
        public string p_channel { get; set; }
        public string p_reg_time_from { get; set; }
        public string p_reg_time_to { get; set; }

        //return
        public int ret_code { get; set; }
        public string ret_msg { get; set; }

    }

    public class SalePortalLeaveMessageByRefferenceNoQuery : IQuery<List<SalePortalLeaveMessageList>>
    {
        public string p_refference_no { get; set; }


        //return
        public int return_code { get; set; }
        public string return_message { get; set; }

    }

    public class SalePortalLeaveMessageByRefferenceNoQuery_IM : IQuery<List<SalePortalLeaveMessageList>>
    {
        public string p_refference_no { get; set; }


        //return
        public int return_code { get; set; }
        public string return_message { get; set; }

    }
}
