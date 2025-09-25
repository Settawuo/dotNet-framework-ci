using System.Collections.Generic;

namespace WBBContract.Commands
{
    public class FBBPurgeDateCommand
    {
        public List<returnErrorFBBPurgeGetDateTBL> iserror { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }
    public class FBBPurgeGetDateTBL
    {
        public decimal CON_ID { get; set; }
        public string CON_TYPE { get; set; }
        public string CON_NAME { get; set; }
    }
    public class returnErrorFBBPurgeGetDateTBL
    {
        public string CON_ID { get; set; }
        public string CON_NAME { get; set; }
        public string CON_TYPE { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }

}
