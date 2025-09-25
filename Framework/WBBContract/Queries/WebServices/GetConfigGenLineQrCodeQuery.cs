using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetConfigGenLineQrCodeQuery : IQuery<GetConfigGenLineQrCodeModel>
    {
        public string MobileNo { get; set; }
        public string FullUrl { get; set; }
    }
}
