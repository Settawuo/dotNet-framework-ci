using System;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class CostInstallationCommand
    {
        public CostInstallationCommand()
        {
            this.RETURN_CODE = -1;
            this.RETURN_DESC = "";
        }
        public ActionType ACTION { get; set; }
        public string SERVICE { get; set; }
        public string VENDOR { get; set; }
        public string ORDER_TYPE { get; set; }
        public Nullable<decimal> RATE { get; set; }
        public Nullable<decimal> PLAYBOX { get; set; }
        public Nullable<decimal> VOIP { get; set; }
        public DateTime EFFECTIVE_DATE { get; set; }
        public Nullable<DateTime> EXPIRE_DATE { get; set; }
        public string REMARK { get; set; }
        public string PHASE { get; set; }

        public Nullable<decimal> RETURN_CODE { get; set; }
        public string RETURN_DESC { get; set; }
    }
}
