//using One2NetBusinessLayer.SFFServices;
using System.Collections.Generic;
using WBBContract.Queries.Commons;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetOnlineQueryAppointmentQuery : IQuery<GetOnlineQueryAppointmentModel>
    {
        public string FullUrl { get; set; }
        public string Internet_No { get; set; }
        public string Transaction_Id { get; set; }
        public string Request_Url { get; set; }

        public OnlineQueryBody Body { get; set; }
        public string BodyJson { get; set; }
    }

    public class OnlineQueryBody
    {
        public string InstallationDate { get; set; }
        public string ProductSpecCode { get; set; }
        public string AccessMode { get; set; }
        public string AddressId { get; set; }
        public string Days { get; set; }
        public string SubDistrict { get; set; }
        public string Postal_Code { get; set; }
        public string SubAccessMode { get; set; }
        public string TaskType { get; set; }
        public List<ASSIGN_CONDITION_ATTR> ASSIGN_CONDITION_LIST { get; set; }
        //R21.3
        public string TimeAdd { get; set; }
        public string ActionTimeSlot { get; set; }
        public string NumTimeSlot { get; set; }

    }
}
