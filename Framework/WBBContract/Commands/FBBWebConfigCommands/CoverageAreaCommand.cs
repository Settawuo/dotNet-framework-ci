namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class CoverageAreaCommand
    {
        public CoverageAreaCommand()
        {
            this.Return_Code = -1;
            this.Return_Desc = "";
        }

        // for return
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }

        //Flag 
        public ActionType ActionType { get; set; }
        public bool FlagDelectAll { get; set; }
        public UpdateCoverageType UpdateCoverageType { get; set; }

        public WBBEntity.PanelModels.FBBWebConfigModels.CoverageAreaPanel CoverageAreaPanel { get; set; }
    }

    public enum UpdateCoverageType
    {
        None = 0,
        SiteInformation = 1,
        SiteCode = 2,
        CoverageInformation = 3
    }

    //public class CoverageArea
    //{
    //    public decimal CVRID { get; set; }
    //    public string CREATED_BY { get; set; }
    //    public System.DateTime CREATED_DATE { get; set; }
    //    public string UPDATED_BY { get; set; }
    //    public System.DateTime? UPDATED_DATE { get; set; }
    //    public string LOCATIONCODE { get; set; }
    //    public string BUILDINGCODE { get; set; }
    //    public string NODENAME_EN { get; set; }
    //    public string NODENAME_TH { get; set; }
    //    public string NODETYPE { get; set; }
    //    public string NODESTATUS { get; set; }
    //    public string ACTIVEFLAG { get; set; }
    //    public decimal? MOO { get; set; }
    //    public string SOI_TH { get; set; }
    //    public string ROAD_TH { get; set; }
    //    public string SOI_EN { get; set; }
    //    public string ROAD_EN { get; set; }
    //    public string ZIPCODE { get; set; }
    //}
}
