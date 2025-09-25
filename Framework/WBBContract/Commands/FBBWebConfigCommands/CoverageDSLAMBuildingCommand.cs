namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class CoverageDSLAMBuildingCommand
    {
        public string Action { get; set; }

        public decimal ContactID { get; set; }

        public string Username { get; set; }
        public decimal CVRID { get; set; }
        public decimal DSLAMID { get; set; }
        public string BuildingUse { get; set; }
        public string BuildingCode { get; set; }

        public string Type { get; set; }
        public string NodeId { get; set; }
        public decimal DSLAMNo { get; set; }

        public decimal CVRRelationID { get; set; }

        public string FlagDup { get; set; }
        public string NodeNameTH { get; set; }
    }
}
