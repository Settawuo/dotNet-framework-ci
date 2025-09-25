using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels
{


    public class Vdsl_fbb_PanelModel
    {
        public string region { get; set; }
        public string regiondisplay { get; set; }
        public string Province { get; set; }
        public string Tumbon { get; set; }
        public string Aumphur { get; set; }
        public string NodeName { get; set; }
        public List<ProvincModel_VDSL> Provincelist { get; set; }
        //public List<TumbonModel_VDSL> TumbonModel_VDSL_List { get; set; }
        //public List<AumphurModel_VDSL> AumphurModel_VDSL_List { get; set; }
        //public List<RegionModel_VDSL> RegionModel_VDSL_List { get; set; }
        //public List<NodeNameTH_VDSL> NodeNameTH_VDSL_List { get; set; }
    }

    public class TumbonModel_VDSL
    {
        public string tumbon { get; set; }
        public string Node_Name { get; set; }
        public DateTime? createdate { get; set; }
        public List<NodeNameTH_VDSL> NodeNamelist { get; set; }
    }

    public class NodeNameTH_VDSL
    {
        public string Node_Name { get; set; }

    }

    public class AumphurModel_VDSL
    {
        public DateTime? createdate { get; set; }
        public string aumphur { get; set; }
        public string Node_Name { get; set; }
        public List<NodeNameTH_VDSL> NodeNamelist { get; set; }
        public List<TumbonModel_VDSL> Tumbonlist { get; set; }

    }

    public class ProvincModel_VDSL
    {
        public string aumphur { get; set; }
        public string province { get; set; }
        public List<AumphurModel_VDSL> Aumphurlist { get; set; }
    }

    public class RegionModel_VDSL
    {
        public string region { get; set; }
        public List<ProvincModel_VDSL> Provincelist { get; set; }
    }

}
