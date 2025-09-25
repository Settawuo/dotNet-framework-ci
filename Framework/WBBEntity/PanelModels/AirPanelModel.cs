using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels
{
    public class AirPanelModel
    {
        public string region { get; set; }
        public string regiondisplay { get; set; }
        public List<ProvincModel> Provincelist { get; set; }
    }

    public class TumbonModel
    {
        public string tumbon { get; set; }
        public DateTime? createdate { get; set; }
    }

    public class AumphurModel
    {
        public string aumphur { get; set; }
        public List<TumbonModel> Tumbonlist { get; set; }
    }

    public class ProvincModel
    {
        public string province { get; set; }
        public List<AumphurModel> Aumphurlist { get; set; }
    }

    public class RegionModel
    {
        public string region { get; set; }
        public List<ProvincModel> Provincelist { get; set; }
    }
}
