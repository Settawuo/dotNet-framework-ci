using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class SMSFlagRegisterPendingQuery : IQuery<SMSFlagRegisterPendingModel>
    {
        public string OrderId { get; set; }
        public string Status { get; set; }
        public string SMS_Flag { get; set; }
        public string Action { get; set; }
        public string Mobile_No { get; set; }
        public string FullUrl { get; set; }
        public string UpdateBy { get; set; }
        public string InternetNo { get; set; }
        public string Option { get; set; }
    }
}
