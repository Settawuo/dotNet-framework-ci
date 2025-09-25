using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class CreateCardPortDataCommand
    {
        public CoveragePort_info CoverageCardPort { get; set; }
        public CoveragePortPanelGrid CoverageCardPortgRIDdata { get; set; }
        public decimal CradNumber { get; set; }
        public string CardModel { get; set; }
        public string CardType { get; set; }
        public string RESERVE_TECHNOLOGY { get; set; }
        public decimal CardModelID { get; set; }
        public decimal CARDID { get; set; }

        public string ResultCommand { get; set; }
        public string Return_Desc { get; set; }
        public decimal Return_Code { get; set; }
        public string Create_BY { get; set; }
        public string Update_BY { get; set; }
        public string ComandBoxdata { get; set; }
        public decimal PORTNUMBER { get; set; }
        public decimal DSalamModelID { get; set; }
        public bool FlagUpdate { get; set; }
        public bool FlagDumg { get; set; }
    }

}
