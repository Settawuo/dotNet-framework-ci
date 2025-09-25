using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class PAYGTransAirnetQuery : IQuery<PAYGTransAirnetListResult>
    {
        public string f_list { get; set; }
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }
    public class PAYGTransAirnetSendFileList
    {
        public string file_name { get; set; }
        public List<string> file_data { get; set; }
    }

}
