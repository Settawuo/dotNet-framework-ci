using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetAppointmentTrackingQuery : IQuery<List<AppointmentDisplayTrackingList>>
    {
        public string order_no { get; set; }
        public string id_card_no { get; set; }
        public string non_mobile_no { get; set; }
        // return code
        public int output_return_code { get; set; }
        public string output_return_message { get; set; }
    }
}
