using System;
using System.Collections.Generic;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class ResendFoaCommand
    {
        public List<FBB_foa_access_list> p_FBB_foa_access_list { get; set; }
        public string ACCESS_NUMBER { get; set; }
        public string IN_XML_FOA { get; set; }
        public string RESEND_STATUS { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }

        public string ret_code { get; set; }
        public string ret_message { get; set; }
    }
    public class FBB_foa_access_list
    {
        public string ACCESS_NUMBER { get; set; }
        public string RESEND_STATUS { get; set; }
    }
}
