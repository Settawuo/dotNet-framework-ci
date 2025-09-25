using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetFBBMeshReportQuery : IQuery<List<FBBMeshReportQueryModel>>
    {
        public string date_to { get; set; }

        public int ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
