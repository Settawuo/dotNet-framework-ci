using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class CreateAWCconfigCommand
    {
        public bool? FlagDup { get; set; }
        public AWCconfig AWCconfigModel { get; set; }
    }
}
