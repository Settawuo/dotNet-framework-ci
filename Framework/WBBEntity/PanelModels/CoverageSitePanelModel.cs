using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.PanelModels
{
    public class CoverageSitePanelModel
    {
        public string TotalSite { get; set; }
        public string TotalCoverage { get; set; }
        public string TotalPort { get; set; }
        public string Available { get; set; }
        public string Active { get; set; }
        public string Reserve { get; set; }
        public string OutOfService { get; set; }
        public string PendingTerminate { get; set; }
        public List<CoverageAreaPanel> CoverageAreaPanel { get; set; }
    }

    public class CoverageAreaPanel
    {
        public string LocationCode { get; set; }
        public string NodeType { get; set; }
        public string NodeNameTH { get; set; }
        public string NodeNameEN { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Province { get; set; }

        public string AmphurTH { get; set; }
        public string TumbonTH { get; set; }
        public string MooTH { get; set; }
        public string SoiTH { get; set; }
        public string RoadTH { get; set; }
        public string ZipCodeTH { get; set; }
        public string AmphurEN { get; set; }
        public string TumbonEN { get; set; }
        public string MooEN { get; set; }
        public string SoiEN { get; set; }
        public string RoadEN { get; set; }
        public string ZipCodeEN { get; set; } 
    }

    public class DslamInfoPanel
    {
        public decimal DSLAMID { get; set; }
        public decimal DSLAMNUMBER { get; set; }
        public decimal DSLAMMODELID { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string NODEID { get; set; }
    }

    public class CardPanel
    {
        public int  Number { get; set; }
        public string Model { get; set; }
        public String CardType { get; set; }
        public string Reserve { get; set; }
        public string NodeId { get; set; }        
    }

    public class PortPanel
    {
        public int PortNumber { get; set; }
        public string PortStatus { get; set; }
        public string PortType { get; set; }
    }
}
