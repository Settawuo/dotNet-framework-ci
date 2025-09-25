using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectDormitoryBuildingQuery : IQuery<List<LovModel>>
    {
        public string State { get; set; }
        public string DormitoryName { get; set; }
    }
    public class SelectAllDormitoryBuildingQuery : IQuery<List<LovModel>>
    {
        public string Province { get; set; }
        public string DormitoryName { get; set; }
        public string BuildingNo { get; set; }
    }
}
