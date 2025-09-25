using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetConfigRedeemCodeQuery : IQuery<RedeemCodeModel>
    {
        public string p_language { get; set; }
        public string p_mobileno { get; set; }
        public string p_idcardno { get; set; }

        public string ClientIP { get; set; }
        public string FullUrl { get; set; }

        // return code
        public string ret_code { get; set; }
        public string ret_message { get; set; }
    }
}
