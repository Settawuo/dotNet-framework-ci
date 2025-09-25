using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels
{
    public class FBSSFixedOM010SendMailFileLogModel
    {
        public string file_name { get; set; }
        public string message_logfile { get; set; }
        public DateTime? modify_date { get; set; }
    }
    public class ReturnFBSSSendMailFileLogBatchModel
    {
        public List<FBSSFixedOM010SendMailFileLogModel> cur { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }

    }
    public class ReturnFBSSOM010Notify
    {
        public string ret_code { get; set; }
        public string msg { get; set; }

    }
}
