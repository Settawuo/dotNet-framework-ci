using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectLovQuery : IQuery<List<LovModel>>
    {
        public string LOV_TYPE { get; set; }
    }

    public class SelectLovByTypeAndLovVal5Query : IQuery<List<LovModel>>
    {
        public string LOV_TYPE { get; set; }
        public string LOV_VAL5 { get; set; }
    }
    public class SelectLovVal5Query : IQuery<List<LovModel>>
    {
        public string LOV_VAL5 { get; set; }
    }
}
