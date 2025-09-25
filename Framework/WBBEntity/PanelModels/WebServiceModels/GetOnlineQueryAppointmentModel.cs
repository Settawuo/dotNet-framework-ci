using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetOnlineQueryAppointmentModel
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
        public string TRANSACTION_ID { get; set; }
        public OnlineQueryDayOffResult DAY_OFF_RESULT { get; set; }
    }

    public class OnlineQueryDayOffResult
    {
        public string ERROR_EVENT { get; set; }
        public List<OnlineQueryTimeSlot> TIME_SLOT_LIST { get; set; }
    }

    public class OnlineQueryTimeSlot
    {
        public string APPOINTMENT_DATE { get; set; }
        public string TIME_SLOT { get; set; }
        public string INSTALLATION_CAPACITY { get; set; }
        public string DAY_OFF_FLAG { get; set; }
        //R21.3
        public string ACTIVE_FLAG { get; set; }
    }
}
