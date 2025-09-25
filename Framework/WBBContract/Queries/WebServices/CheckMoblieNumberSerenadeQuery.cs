using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckMobileNumberSerenadeQuery : IQuery<CheckMobileNumberSerenadeModel>
    {
        public string MobileNo { get; set; }
        // Update 17.6
        public string FullUrl { get; set; }
    }
}
