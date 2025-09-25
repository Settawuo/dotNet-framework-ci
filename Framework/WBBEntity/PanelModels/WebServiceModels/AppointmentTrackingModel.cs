using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class AppointmentTrackingModel
    {
        public List<AppointmentDisplayTrackingList> DisplayTrackingList { get; set; }
    }

    public class AppointmentDisplayTrackingList
    {
        public int output_return_code { get; set; }
        public string output_return_message { get; set; }

        public DateTime? create_date_zte { get; set; }
        public string appointment_date { get; set; }
        public string appointment_timeslot { get; set; }
    }
}
