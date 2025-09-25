using System;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class DSLAMInfoCommand
    {
        public DSLAMInfoCommand()
        {
            this.Return_Code = -1;
            this.Return_Desc = "err";
        }

        // for return
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }

        // Properties
        public decimal DSLAMID { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public decimal DSLAMNUMBER { get; set; }
        public decimal DSLAMMODELID { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string NODEID { get; set; }
        public string REGION_CODE { get; set; }
        public string LOT_NUMBER { get; set; }
    }
}
