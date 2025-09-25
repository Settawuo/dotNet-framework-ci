using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class EvOmPlayboxExtensionInfoModel
    {
        public string CountPlaybox { get; set; }
        public string ErrorMessage { get; set; }
        public string MainPlayboxCode { get; set; }
        public string MainPlayboxName { get; set; }
        public string MainPlayboxExists { get; set; }
        public List<UsePlaybox> UsePlayboxList { get; set; }
        public List<AvailPlaybox> AvailPlayboxList { get; set; }
    }

    public class UsePlaybox
    {
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string Serial { get; set; }
    }

    public class AvailPlaybox
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int PlayBoxExt { get; set; }
    }
}
