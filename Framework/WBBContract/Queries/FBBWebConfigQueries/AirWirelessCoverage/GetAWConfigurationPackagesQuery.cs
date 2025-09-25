using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage
{
    public class GetAWConfigurationPackagesQuery : IQuery<List<NewPackageMaster>>
    {
        public string PromotionCode { get; set; }
        public string PromotionNameThai { get; set; }
        public string PromotionNameEng { get; set; }
    }
}
