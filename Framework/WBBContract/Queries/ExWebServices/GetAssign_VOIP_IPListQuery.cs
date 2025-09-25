using System;

namespace WBBContract.Queries.ExWebServices
{
    public class GetAssign_VOIP_IPListQuery
    {


        public string IP_ADDRESS { get; set; }
        public string REFF_USER { get; set; }
        public string REFF_KEY { get; set; }
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
        public string FlaxData { get; set; }
        public String Port { get; set; }
    }
}
