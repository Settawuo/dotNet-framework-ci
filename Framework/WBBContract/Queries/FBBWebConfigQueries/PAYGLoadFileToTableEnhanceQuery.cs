using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class PAYGLoadFileToTableEnhanceQuery : IQuery<PAYGLoadFileToTableEnhanceListResult>
    {
        public string flag_check { get; set; }


        //public List<PAYGLoadFileToTableEnhanceQueryList> PAYGLoadFileToTableEnhanceModels { get; set; }
        public List<PAYGLoadFileToTableEnhanceFileList> loadfile_enhance_list { get; set; }
        //return value
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }


    public class PAYGLoadFileToTableEnhanceSendFileList
    {
        public string file_name { get; set; }
        public string file_index { get; set; }
        public List<string> file_data { get; set; }
    }
}
