using System;

namespace WBBEntity.PanelModels.StoredProc
{
    public class TimeSlotModel
    {
        public DateTime Appointment_Date { get; set; }
        public string Time_Slot { get; set; }
        public string Installation_Capacity { get; set; }
        public decimal Time_Slot_Id { get; set; }
    }
}
