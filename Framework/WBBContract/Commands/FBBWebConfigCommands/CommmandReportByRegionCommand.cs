using System;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class CommmandReportByRegionCommand
    {
        public CommmandReportByRegionCommand()
        {
            this.Return_Code = -1;
            this.Return_Desc = "";
        }
        // for return
        public string REPORT_CODE { get; set; }
        public string REPORT_NAME { get; set; }
        public string REPORT_DESC { get; set; }
        public string REPORT_STATUS { get; set; }
        public string REPORT_PARAMETER { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string Update_BY { get; set; }
        public string Des_Code { get; set; }
        public string Flag_Add_log_Rpt { get; set; }
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }


    }
}
