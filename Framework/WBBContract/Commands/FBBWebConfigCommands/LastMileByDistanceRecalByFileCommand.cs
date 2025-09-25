using System.Collections.Generic;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class LastMileByDistanceRecalByFileCommand
    {
        public List<LastMileRecal> p_recal_access_list { get; set; }
        public string p_USER { get; set; }
        public string p_STATUS { get; set; }
        public string p_filename { get; set; }
        //return
        public string messege_log_file { get; set; }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
