using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class StandardAddressModel
    {
    }

    public class StdAddressFullConModel
    {
        public string OltNo { get; set; }
        public string Region { get; set; }
    }
    public class StdAddressFullConListResult
    {
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
        public List<StdAddressFullConList> Data { get; set; }
    }
    public class StdAddressFullConList
    {
        public string REGION { get; set; }
        public string OLT_NO { get; set; }
        public string OLT_PORT_OUT { get; set; }
        public string ODF_NO { get; set; }
        public string ODF_PORT_OUT { get; set; }
        public string SITE_NO { get; set; }
        public string SP1_NO { get; set; }
        public string SP1_PORT_OUT { get; set; }
        public decimal? SP1_LATITUDE { get; set; }
        public decimal? SP1_LONGITUDE { get; set; }
        public string SP2_NO { get; set; }
        public decimal? AVAILABLE_PORT { get; set; }
        public decimal? USED_PORT { get; set; }
        public string SP2_ALIAS { get; set; }
        public decimal? SP2_LATITUDE { get; set; }
        public decimal? SP2_LONGITUDE { get; set; }
        public string ADDRESS_ID { get; set; }
        public string ADDR_NAME_EN { get; set; }
        public string ADDR_NAME_TH { get; set; }
        public string SP2_CREATED_DATE { get; set; }
        //public string SITE_NO { get; set; }
        //public string AVAILABLE_PORT { get; set; }
        //public string USED_PORT { get; set; }


    }
}
