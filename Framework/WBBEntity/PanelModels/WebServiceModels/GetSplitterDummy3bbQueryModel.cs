using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class GetSplitterDummy3bbQueryModel
    {
        public string ReturnCode { get; set; } = "-1";
        public string ReturnMsg { get; set; } = string.Empty;
        public List<GetSplitterDummy3bbQueryModelData> Data { get; set; } = new List<GetSplitterDummy3bbQueryModelData>();
    }

    public class GetSplitterDummy3bbQueryModelData
    {
        public string splitter_no { get; set; }
    }

}
