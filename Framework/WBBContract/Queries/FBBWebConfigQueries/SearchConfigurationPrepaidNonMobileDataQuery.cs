using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SearchConfigurationPrepaidNonMobileDataQuery : IQuery<List<ConfigurationPrepaidNonMobileData>>
    {

        public string DormitoryName { get; set; }
        public string BuildingNo { get; set; }
        public string DormitoryProvince { get; set; }
        public string Stutus { get; set; }
        public string User { get; set; }

        public string FibrenetID { get; set; }
        public string Region { get; set; }
        public string FloorNo { get; set; }
        public string RoomNo { get; set; }
    }
}
