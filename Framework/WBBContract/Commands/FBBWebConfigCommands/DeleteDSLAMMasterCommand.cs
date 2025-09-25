namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class DeleteDSLAMMasterCommand
    {
        public string Lot { get; set; }
        public string RegionCode { get; set; }
        public string Username { get; set; }
        public bool? FlagNot { get; set; }
        public bool? FlagUpdate { get; set; }
        public decimal Loop { get; set; }
        public decimal OldNo { get; set; }
        public decimal NewNo { get; set; }
    }
}
