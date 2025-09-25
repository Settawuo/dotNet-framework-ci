using System.Collections.Generic;

namespace WBBEntity.FBSSModels
{
    public class CheckPremiumFlagModel
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public string SubAccessMode { get; set; }
        public string RecurringCharges { get; set; }
        public string LocationCodes { get; set; }
        public string AccessModes { get; set; }
        public List<ReturnPremiumTimeSlotModel> ReturnPremiumTimeSlot { get; set; }
    }

    public class ReturnPremiumTimeSlotModel
    {
        public string PartnerSubtype { get; set; }
        public string AccessMode { get; set; }
        public string numberofday { get; set; }
        public string numberofhour { get; set; }
        public string shifttype { get; set; }
        public string numberofshift { get; set; }
        public string assignrule { get; set; }
        public string numberofdisplay { get; set; }
    }
}
