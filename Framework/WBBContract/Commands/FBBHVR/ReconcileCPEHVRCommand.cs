using System;
using System.Collections.Generic;
namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class ReconcileCPEHVRCommand
    {
        public List<ReconcileCPEHVRReturn> RET_CUR_FILE { get; set; }
        public string RET_CODE { get; set; }
        public string RET_MSG { get; set; }
    }

    public class ReconcileCPEHVRReturn
    {
        public string output_path { get; set; }
        public string output_filename { get; set; }
        public string data_file { get; set; }
    }
}
