namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class CoverageAreaBuildingCommand
    {
        public CoverageAreaBuildingCommand()
        {
            this.Return_Code = -1;
            this.Return_Desc = "err";
        }

        // for return
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }

        // Properties
        public decimal CONTACT_ID { get; set; }
        public string CREATED_BY { get; set; }

        public string BUILDING { get; set; }
        public string BUILDING_EN { get; set; }
        public string BUILDING_TH { get; set; }
        public string INSTALLNOTE { get; set; }
        public string RefKey { get; set; }

        //Flag 
        public ActionType ActionType { get; set; }
    }

}
