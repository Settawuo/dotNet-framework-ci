using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{


    public class SelectFTTXFBBQuery : IQuery<List<Fttx_Fbb_PanelModel>>
    {
        public string region { get; set; }
        public string province { get; set; }
        public string tumbon { get; set; }
        public string aumphur { get; set; }
        public string ResultCheckData { get; set; }
        public string GROUP_AMPHUR { get; set; }


    }
}
