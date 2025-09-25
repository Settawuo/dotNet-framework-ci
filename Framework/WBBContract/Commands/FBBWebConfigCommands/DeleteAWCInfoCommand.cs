using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class DeleteAWCInfoCommand
    {
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public decimal APP_ID { get; set; }
        public List<AWCconfig> model { get; set; }
    }
}
