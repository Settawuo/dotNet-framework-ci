using WBBEntity.PanelModels;

namespace WBBContract.Commands
{
    public class InsertCloudIPCameraCommand : CommandBase
    {
        private QuickWinPanelModel _QuickWinPanelModel;
        public QuickWinPanelModel QuickWinPanelModel
        {
            get { return _QuickWinPanelModel ?? (_QuickWinPanelModel = new QuickWinPanelModel()); }
            set { _QuickWinPanelModel = value; }
        }

        public string p_cust_row_id { get; set; }
        public string p_register_flag { get; set; }
        public string p_package_service_code { get; set; }
        public string p_product_subtype { get; set; }
        public string p_package_type { get; set; }
        public string p_package_code { get; set; }
        public decimal? p_package_price { get; set; }
        public decimal? p_package_count { get; set; }
        public string p_return_order { get; set; }
        public string p_created_by { get; set; }
        public string p_fibrenet_id { get; set; }
        public System.DateTime? p_created_date { get; set; }
        public string p_updated_by { get; set; }
        public System.DateTime? p_updated_date { get; set; }
        public decimal? ret_code { get; set; }
        public string ret_msg { get; set; }
    }
}
