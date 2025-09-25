using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class RedeemCodeModel
    {
        public RedeemCodeModel()
        {
            if (list_config_redeem_code == null)
                list_config_redeem_code = new List<RedeemCodeConfigDataModel>();
        }

        public string ret_code { get; set; }
        public string ret_message { get; set; }
        public List<RedeemCodeConfigDataModel> list_config_redeem_code { get; set; }

    }

    public class RedeemCodeConfigDataModel
    {
        public string game_name { get; set; }
        public byte[] game_item_img { get; set; }
        public string item_name { get; set; }
        public string item_price { get; set; }
        public string item_detail { get; set; }
        public string url_redeem { get; set; }
        public string sms_message { get; set; }
        public string url_req_privilege_barcode { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string ip_address { get; set; }
        public string short_code { get; set; }
        public string req_privilege_barcode { get; set; }
        public byte[] game_popup_img { get; set; }

        public string game_item_img_base64 { get; set; }
        public string game_popup_img_base64 { get; set; }
    }

    public class PersonalInfoRedeemCodeModel
    {
        public string outMessage { get; set; }
        public string outContactMobileNo { get; set; }
        public string descEng { get; set; }
        public string descThai { get; set; }
        public string inStatementEng { get; set; }
        public string inStatementThai { get; set; }
        public string monthlyFee { get; set; }
        public string paymentMode { get; set; }
        public string priceExclVat { get; set; }
        public string priceType { get; set; }
        public string productCd { get; set; }
        public string productClass { get; set; }
        public string productPkg { get; set; }
        public string productSeq { get; set; }
        public string produuctGroup { get; set; }
        public string promotionName { get; set; }
        public string shortNameEng { get; set; }
        public string shortNameThai { get; set; }
    }
}
