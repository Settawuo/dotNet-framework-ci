using System;
using System.Collections.Generic;

namespace WBBContract.Queries.FBBWebConfigQueries
{

    public class SelectPanelDataVDSL
    {
        public string region { get; set; }
        public string regiondisplay { get; set; }
        public string Province { get; set; }
        public string Aumphur { get; set; }
        public string Region { get; set; }
        public string Tumbon { get; set; }
        public string Tower { get; set; }

        public string Service_Type { get; set; }

        public List<ProvincModel_VDSL_T> Provincelist { get; set; }
    }

    public class TumbonModel_VDSL_T
    {
        public string tumbon { get; set; }
        public DateTime? createdate { get; set; }
    }

    public class AumphurModel_VDSL_T
    {
        public string aumphur { get; set; }
        public string Tower_TH { get; set; }
        public List<TumbonModel_VDSL_T> Tumbonlist { get; set; }
    }

    public class ProvincModel_VDSL_T
    {
        public string province { get; set; }

        public List<AumphurModel_VDSL_T> Aumphurlist { get; set; }
    }

    public class RegionModel_VDSL_T
    {
        public string region { get; set; }
        public List<ProvincModel_VDSL_T> Provincelist { get; set; }
    }


}
