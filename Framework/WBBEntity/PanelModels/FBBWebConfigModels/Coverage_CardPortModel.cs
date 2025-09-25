using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    class Coverage_CardPortModel
    {

        public decimal Number { get; set; }
        public string CardModel { get; set; }
        public string CardType { get; set; }
        public string Reseve { get; set; }
        public decimal CardModelID { get; set; }
        public decimal DSalamModelID { get; set; }
        public List<CoveragePortPanelGrid> CoveragePortPanelGrid { get; set; }

        public List<CoveragePortPanel> CoveragePortPanel { get; set; }
        public List<portPanelHittory> PortPanelHitory { get; set; }
        public List<CoveragePort_info> CoveragePort_info { get; set; }
    }


    public class CoveragePortPanelGrid
    {

        public decimal Number { get; set; }
        public string CardModel { get; set; }
        public decimal CARModelID { get; set; }
        public string CardType { get; set; }
        public string Reseve { get; set; }
        public decimal CARDID { get; set; }
        public decimal DSalamModelID { get; set; }
        public decimal Maxslot { get; set; }
        public string Nodeid { get; set; }
        public bool HasPort { get; set; }
        public bool HasPortGenarent { get; set; }
        public string CvrID { get; set; }
        public int Maxtabel { get; set; }
        public string NodeID { get; set; }
        public string Building { get; set; }
        public bool ChageTie { get; set; }


    }


    public class CoveragePort_info
    {


        public decimal PORTID { get; set; }
        public decimal CARDID { get; set; }
        public decimal PORTNUMBER { get; set; }
        public Nullable<decimal> PORTSTATUSID { get; set; }
        public string PORTTYPE { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }




    }
    public class CoveragePortPanel
    {
        public decimal PortID { get; set; }
        public decimal PortNumber { get; set; }
        public string PortStatus { get; set; }
        public string PortType { get; set; }




    }
    public class portPanelHittory
    {
        public decimal PortID { get; set; }
        public DateTime Datetime { get; set; }
        public string User { get; set; }
        public string DESC { get; set; }



    }

}
