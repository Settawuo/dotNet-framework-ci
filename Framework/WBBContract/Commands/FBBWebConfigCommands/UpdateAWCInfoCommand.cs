using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class UpdateAWCInfoCommand
    {
        public bool? FlagDup { get; set; }
        public string dupname { get; set; }
        public AWCinformation awcmodel { get; set; }
        public List<AWCconfig> awcmodelconfig { get; set; }
        public string oldbasename { get; set; }
        public string oldsitename { get; set; }
    }
}
