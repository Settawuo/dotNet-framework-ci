namespace WBBContract.Commands
{
    public class ReportActiveCommand
    {
        public ReportActiveCommand()
        {
            this.Return_Code = -1;
            this.Return_Desc = "";
        }

        public string Create_By { get; set; }
        public string Report_Code { get; set; }

        public string ReportParam { get; set; }

        // for return
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
    }
}
