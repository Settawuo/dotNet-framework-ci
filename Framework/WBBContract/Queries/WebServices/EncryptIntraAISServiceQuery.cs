using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class EncryptIntraAISServiceQuery : IQuery<EncryptIntraAISServiceModel>
    {
        public string Url { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string p_transaction_id { get; set; }
        public string p_non_mobile_no { get; set; }
        public EncryptIntraAISServiceBody body { get; set; }
    }

    public class EncryptIntraAISServiceBody
    {
        public string ssid { get; set; }
        public string command { get; set; }
        public string input { get; set; }
    }
}
