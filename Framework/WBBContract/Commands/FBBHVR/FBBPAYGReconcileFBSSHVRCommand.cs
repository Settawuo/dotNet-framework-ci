using System.Collections.Generic;
namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class FBBPAYGReconcileFBSSHVRCommand
    {
        public string p_report_name { get; set; }
        //Return 
        public List<FBBPAYGReconcileFBSSHVRReturn> RET_CUR_FILE { get; set; }
        public string RET_CODE { get; set; }
        public string RET_MSG { get; set; }
    }

    public class FBBPAYGReconcileFBSSHVRReturn
    {
        public string output_path { get; set; }
        public string output_filename { get; set; }
        public string data_file { get; set; }
    }
}
