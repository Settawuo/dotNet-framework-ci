using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class ConfirmOneTimePWQuery : IQuery<GssoSsoResponseModel>
    {
        public string msisdn { get; set; }
        public string pwd { get; set; }
        public string transactionID { get; set; }
    }

    public class SendOneTimePWQuery : IQuery<GssoSsoResponseModel>
    {
        public string msisdn { get; set; }
        public string accountType { get; set; }
        public bool use3BBService { get; set; }
    }
}
