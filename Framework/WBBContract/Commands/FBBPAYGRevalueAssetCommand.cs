using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBContract.Commands
{
    public class FBBPAYGRevalueAssetCommand
    {
        public List<returnErrorFBBPAYGRevalueAssetTBL> iserror { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public string date_start { get; set; }
        public string date_to { get; set; }


        public List<string> RevalueAssetDataReturn { get; set; }
        public List<string> RevalueAssetData { get; set; }
    }
    public class FBBPAYGRevalueAssetGetDateTBL
    {
        public decimal CON_ID { get; set; }
        public string CON_TYPE { get; set; }
        public string CON_NAME { get; set; }
    }
    public class returnErrorFBBPAYGRevalueAssetTBL
    {
        public string CON_ID { get; set; }
        public string CON_NAME { get; set; }
        public string CON_TYPE { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
