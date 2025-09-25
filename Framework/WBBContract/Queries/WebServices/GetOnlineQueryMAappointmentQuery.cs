using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetOnlineQueryMAappointmentQuery : IQuery<GetOnlineQueryMAappointmentModel>
    {
        public string FullUrl { get; set; }
        public string Internet_No { get; set; }
        public string Transaction_Id { get; set; }
        public string Request_Url { get; set; }

        public OnlineQueryMABody Body { get; set; }
        public string BodyJson { get; set; }
    }

    public class OnlineQueryMABody
    {
        public string AccessNo { get; set; }
        public string MADate { get; set; }
        public string AccessMode { get; set; }
        public string Days { get; set; }
        public string SymptomCode { get; set; }
        public string ServiceLevel { get; set; }
        public string TimeAdd { get; set; }
        public string ActionTimeSlot { get; set; }
        public string NumTimeSlot { get; set; }
    }
}
