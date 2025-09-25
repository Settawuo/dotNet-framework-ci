using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class UpdateFbssConfigTblWithOMCommand
    {
        public string con_type { get; set; }
        public string con_name { get; set; }
        public string display_val { get; set; }
        public string val1 { get; set; }
        public string val2 { get; set; }
        public string flag { get; set; }
        public string updated_by { get; set; }
    }
    public class UpdateFbssConfigTblWithOM_Result
    {
        public string ret_code { get; set; }
        public string ret_message { get; set; }
    }
}
