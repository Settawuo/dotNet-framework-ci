using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBContract.Commands
{
    public class TranMatDocLoadFileLogCommand
    {
        //Return 
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }

    public class InsertTranMatDocLoadFileLogCommand : TranMatDocLoadFileLogCommand
    {
        public string filename { get; set; }
        public DateTime filedate { get; set; }
        public string message { get; set; }
        public string flag_type { get; set; }
    }
}
