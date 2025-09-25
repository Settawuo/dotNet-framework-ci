using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class FBBTransactionWorkflowModel
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<String> ReturnCursor { get; set; }
    }
}
