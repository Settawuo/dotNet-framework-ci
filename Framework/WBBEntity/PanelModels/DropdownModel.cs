using System.Collections.Generic;

namespace WBBEntity.PanelModels
{
    public class DropdownModel
    {
        public string Text { get; set; }
        public string Value { get; set; }

        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
        public string Value5 { get; set; }

        public string DefaultValue { get; set; }

        public decimal ID { get; set; }
    }

    public class RoutherRegisModel
    {
        public List<DropdownModel> CardType { get; set; }
        public List<DropdownModel> Dormitory { get; set; }
    }

}
