using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectDormitoryNameQuery : IQuery<List<LovModel>>
    {
        public string State { get; set; }
        public string DormitoryName { get; set; }
        public string Region { get; set; }

    }

    public class SelectAllDormitoryNameQuery : IQuery<List<LovModel>>
    {
        public string Province { get; set; }
        public string Region { get; set; }
    }
}
