using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class RequestsGSSOServiceQuery : IQuery<RequestsGSSOServiceModel>
    {
        public string p_endpoint { get; set; }
        public string p_mobile_no { get; set; }
        public string p_bodyJsonStr { get; set; }
        public string FullUrl { get; set; }
    }
}
