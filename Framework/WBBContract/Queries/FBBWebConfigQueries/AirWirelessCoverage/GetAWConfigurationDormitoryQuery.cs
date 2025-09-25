using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage
{
    public class GetAWConfigurationDormitoryQuery : IQuery<List<ConfigurationDormitoryData>>
    {
        public string DormitoryProvince { get; set; }
        public string DormitoryName { get; set; }
        public string Region { get; set; }
    }

    public class GetAWConfigurationDormitoryByIDQuery : IQuery<ConfigurationDormitoryModel>
    {
        public string p_dormitory_id { get; set; }
    }
}
