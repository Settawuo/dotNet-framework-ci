using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class CreateAWCInfoCommand
    {
        public bool? FlagDup { get; set; }
        public string dupname { get; set; }
        public AWCinformation awcmodel { get; set; }
        public List<AWCconfig> awcmodelconfig { get; set; }
    }
}
