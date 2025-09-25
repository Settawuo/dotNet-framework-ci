using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class UpdateDocumentOrCSNoteByNotifyOrderResponse
    {
        public List<ReturnListFileNameData> ReturnListFileName { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }

    public class ReturnListFileNameData
    {
        public string FileName { get; set; }
    }
}
