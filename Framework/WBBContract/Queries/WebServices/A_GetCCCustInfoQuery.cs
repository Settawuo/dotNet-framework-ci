using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class A_GetCCCustInfoQuery : IQuery<A_GetCCCustInfoModel>
    {
        public string MOBILE_NO { get; set; }
        public string BROWSER_TYPE { get; set; }
        public string DEVICE_TYPE { get; set; }
        public string TRANSACTION_ID { get; set; }

        // Update 17.5
        public string FullUrl { get; set; }
    }
}
