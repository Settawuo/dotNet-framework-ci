using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class RelatedMobileServiceATNQuery : IQuery<RelatedMobileServiceATNModel>
    {
        public string channel { get; set; }
        public string username { get; set; }
        public string idCardNo { get; set; }
        public string mobileNo { get; set; }
        public string mode { get; set; }
        public string languageCode { get; set; }
    }
}
