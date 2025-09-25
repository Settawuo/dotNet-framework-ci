using WBBEntity.FBBHVRModels;
using WBBEntity.FBBShareplexModels;
using WBBEntity.PanelModels.ShareplexModels;

namespace WBBContract.Queries.FBBHVR
{
    public class GetPremiumAreaHVRQuery : IQuery<PremiumAreaModel>
    {
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Language { get; set; }
        public string ClientIP { get; set; }
        public string FullUrl { get; set; }

        // return code
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }
}
