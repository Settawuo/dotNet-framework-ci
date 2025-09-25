using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels
{


    public class Fttx_Fbb_PanelModel
    {
        public string region { get; set; }
        public string regiondisplay { get; set; }
        public string Province { get; set; }
        public string Aumphur { get; set; }
        ///public string Region { get; set; }
        public string Tumbon { get; set; }

        public string Service_Type { get; set; }

        public List<ProvincModel_Fttx> Provincelist { get; set; }
    }

    public class TumbonModel_Fttx
    {
        public string tumbon { get; set; }
        public string Tower_TH { get; set; }
        public List<TumbonModel_Fttx> Tumbonlist { get; set; }
        public List<Towerth_Fttx> Towerlist { get; set; }

        public DateTime? createdate { get; set; }
    }



    public class Towerth_Fttx
    {

        public string Tower_TH { get; set; }

    }

    public class AumphurModel_Fttx
    {
        public DateTime? createdate { get; set; }
        public string aumphur { get; set; }
        public string Tower_TH { get; set; }
        public string tumbon { get; set; }
        public List<TumbonModel_Fttx> Tumbonlist { get; set; }
        public List<Towerth_Fttx> Towerlist { get; set; }

    }


    public class ProvincModel_Fttx
    {
        public string province { get; set; }
        public string aumphur { get; set; }
        public List<AumphurModel_Fttx> Aumphurlist { get; set; }
        public string Tower_th { get; set; }
    }



    public class RegionModel_Fttx
    {
        public string region { get; set; }
        public List<ProvincModel_Fttx> Provincelist { get; set; }
    }



}
