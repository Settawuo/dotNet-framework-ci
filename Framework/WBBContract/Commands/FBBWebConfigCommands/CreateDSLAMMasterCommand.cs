using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class CreateDSLAMMasterCommand
    {
        public DSLAMMasterModel DSLAMMasterModel { get; set; }
        public bool? FlagDup { get; set; }
        public bool? FlagUpdate { get; set; }
        public decimal OldNo { get; set; }
        public decimal NewNo { get; set; }
    }
}
