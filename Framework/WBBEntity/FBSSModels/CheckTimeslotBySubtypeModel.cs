using System.Collections.Generic;

namespace WBBEntity.FBSSModels
{
    public class CheckTimeslotBySubtypeModel
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<ReturnConfigTimeslotModel> ReturnConfigTimeslot { get; set; }
    }

    public class ReturnConfigTimeslotModel
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
