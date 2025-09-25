using System;
using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class AWCModel
    {
        public string region { get; set; }
        public string province { get; set; }
        public string aumphur { get; set; }
        public string tumbon { get; set; }
        public string APname { get; set; }
        public List<AWCSearchlist> awcsearchresult { get; set; }
    }

    public class AWCSearchlist
    {
        public string province { get; set; }
        public string aumphur { get; set; }
        public string tumbon { get; set; }
        public string APname { get; set; }
        public decimal? TotalCoverage { get; set; }
        public decimal? TotalAP { get; set; }
        public decimal? ap_id { get; set; }
        public decimal? app_id { get; set; }
        public decimal? site_id { get; set; }
        public DateTime? updatedate { get; set; }

    }

    public class AWCexportlist
    {
        public string AP_Name { get; set; }
        public string Sector { get; set; }
        public string Base_L2 { get; set; }
        public string Site_Name { get; set; }
        public string Aumphur { get; set; }
        public string Tumbon { get; set; }
        public string Province { get; set; }
        public string Zone { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public decimal AP_ID { get; set; }
        public decimal APP_ID { get; set; }
        public string ACTIVE_FLAGINFO { get; set; }
        public string ACTIVE_FLAGAPPC { get; set; }

        //new//

        public string Tower_Type { get; set; }
        public string Tower_Height { get; set; }
        public string VLAN { get; set; }
        public string Subnet_Mask_26 { get; set; }
        public string Gateway { get; set; }
        public string Comment { get; set; }

        public string IP_Address { get; set; }
        public string Status { get; set; }
        public string Implement_Phase { get; set; }
        public string Implement_date { get; set; }
        public string Onservice_date { get; set; }
        public string PO_Number { get; set; }
        public string AP_Company { get; set; }
        public string AP_Lot { get; set; }



    }

    public class AWCexportResultlist
    {
        public string AP_Name { get; set; }
        public string Sector { get; set; }
        public string Base_L2 { get; set; }
        public string Site_Name { get; set; }
        public string Aumphur { get; set; }
        public string Tumbon { get; set; }
        public string Province { get; set; }
        public string Zone { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }

        public string VLAN { get; set; }
        public string subnet_mask_26 { get; set; }
        public string gateway { get; set; }
        public string ap_comment { get; set; }
        public string tower_type { get; set; }
        public string tower_height { get; set; }

        public string IP_Address { get; set; }
        public string Status { get; set; }
        public string Implement_Phase { get; set; }
        public DateTime? Implement_date { get; set; }
        public DateTime? On_service_date { get; set; }
        public string PO_Number { get; set; }
        public string AP_Company { get; set; }
        public string AP_Lot { get; set; }

    }

    public class AWCconfig
    {
        public string AP_Name { get; set; }
        public string Sector { get; set; }
        public decimal AP_ID { get; set; }
        public decimal Site_id { get; set; }
        public string ACTIVE_FLAGINFO { get; set; }
        public string user { get; set; }
        public DateTime? updatedate { get; set; }

        ///// new //////

        public string ip_address { get; set; }
        public string status { get; set; }
        public string implement_phase { get; set; }
        public DateTime? onservice_date { get; set; }
        public string po_number { get; set; }
        public string ap_company { get; set; }
        public string ap_lot { get; set; }
        public DateTime? implement_date { get; set; }


    }
    public class AWCinformation
    {
        public string Base_L2 { get; set; }
        public string Site_Name { get; set; }
        public string Aumphur { get; set; }
        public string Tumbon { get; set; }
        public string Province { get; set; }
        public string Zone { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public decimal APP_ID { get; set; }
        public string ACTIVE_FLAGAPPC { get; set; }
        public string user { get; set; }
        public List<AWCconfig> apmodel { get; set; }
        public AWCModel oldmodelpage1 { get; set; }
        /// <summary>
        /// new
        /// </summary>
        public string VLAN { get; set; }
        public string subnet_mask_26 { get; set; }
        public string gateway { get; set; }
        public string ap_comment { get; set; }
        public string tower_type { get; set; }
        public string tower_height { get; set; }


    }

    public class Dupfile
    {
        public bool dup { get; set; }
        public decimal result { get; set; }
    }

}
