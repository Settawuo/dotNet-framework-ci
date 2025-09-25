using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WBBContract.Commands
{
    public class LoadSimS4Command
    {
        //Return 
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }

    public class LoadSimS4DataInsertCommand : LoadSimS4Command
    {
        public string psp_file { get; set; }
        public string pt_name { get; set; }
    }

    public class LoadSimS4DataUpdateFlagCommand : LoadSimS4Command
    {
        public string flag_fbss { get; set; }
        public string serial { get; set; }
        public string status { get; set; }
    }

    public class InsertGenLogCommand
    {
        public string pfn_name { get; set; }
        public string pf_name { get; set; }
        public string pt_name { get; set; }
        public string pin_xml { get; set; }
        public string pout_ret { get; set; }
        public string pexc_det { get; set; }
        public string pout_xml { get; set; }
        public string prow_cnt { get; set; }
        public string pret_msg {get; set; }
    }

    public class InsertLoadFileLogCommand : LoadSimS4Command
    {
        public string filename { get; set; }
        public DateTime filedate { get; set; }
        public string message { get; set; }
        public string flag_type { get; set; }
    }

    public class ConnectionConfig
    {
        public string username { get; set; }
        public string password { get; set; }
        public string domain { get; set; }
        public string fullpath { get; set; }
    }
}
