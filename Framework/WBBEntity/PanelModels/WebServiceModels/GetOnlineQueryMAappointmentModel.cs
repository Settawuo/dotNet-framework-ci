using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetOnlineQueryMAappointmentModel
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
        public string TRANSACTION_ID { get; set; }
        public OnlineQueryMADayOffResult DAY_OFF_RESULT { get; set; }
    }

    public class OnlineQueryMADayOffResult
    {
        public string ERROR_EVENT { get; set; }
        public List<OnlineQueryMATimeSlot> TIME_SLOT_LIST { get; set; }
    }

    public class OnlineQueryMATimeSlot
    {
        public string MA_DATE { get; set; }
        public string MA_TIME_SLOT { get; set; }
        public string CAPACITY { get; set; }
        public string DAY_OFF_FLAG { get; set; }
    }
}
