using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class ServiceLogNewRegisterQuery : IQuery<ServiceLogNewRegisterModel>
    {
        public string OrderNo { get; set; }
        public string ServiceName { get; set; }
    }
}
