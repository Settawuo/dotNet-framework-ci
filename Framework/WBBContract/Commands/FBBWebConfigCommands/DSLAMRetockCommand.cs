namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class DSLAMRetockCommand
    {
        public string Action { get; set; }
        public string Username { get; set; }
        public string NodeID { get; set; }
        public string NodeTH { get; set; }
        public decimal DSLAMID { get; set; }
        public bool? FlagNot { get; set; }
    }
}
