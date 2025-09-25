using System.Collections.Generic;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class GendatafileExpireSIMCommand
    {
        //Input
        public string in_report_name { get; set; }

        //Output
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public List<listReturnData> return_report_data_list_cur { get; set; }
    }
    public class listReturnData
    {
        public string FILE_NAME { get; set; }
        public string FILE_DATA { get; set; }

    }
}
