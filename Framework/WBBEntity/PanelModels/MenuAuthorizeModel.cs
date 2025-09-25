using System.Collections.Generic;

namespace WBBEntity.PanelModels
{
    public class MenuAuthorizeModel
    {
        public List<MenuAuthorize> ListMenuAuthorize { get; set; }
        public decimal return_code { get; set; }
        public string return_message { get; set; }

    }

    public class MenuAuthorize
    {
        public string menu_code { get; set; }
        public string parent_menu_code { get; set; }
        public string menu_name_th { get; set; }
        public string menu_name_en { get; set; }
        public string menu_url { get; set; }
        public string menu_authorize { get; set; }
        public decimal order_by { get; set; }
    }

    public class PackageTopupInternetNotUseModel
    {
        public List<PackageTopupInternetNotUse> ListPackageTopupInternetNotUse { get; set; }
        public string return_code { get; set; }
        public string return_message { get; set; }
    }

    public class PackageTopupInternetNotUse
    {
        public string sff_promotion_code { get; set; }
    }
}
