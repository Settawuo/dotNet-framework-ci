using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class PAYGEnhanceMatDocQuery : IQuery<PAYGTransAirnetListResult>
    {
        public string f_list { get; set; }
        public List<PAYGTransAirnetFileList> f_enchance_list { get; set; }
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }
}
