using System.Collections.Generic;

namespace WBBEntity.PanelModels
{
    public class PdfOnWebModel
    {
        public decimal return_code { get; set; }
        public string return_message { get; set; }
        public List<PdfOnWebData> LIST_PDF_CUR { get; set; }
    }

    public class PdfOnWebData
    {
        public string MAIN_PACKAGE { get; set; }
        public string ONTOP_WIFI_ROUTER { get; set; }
        public string ONTOP_FIXEDLINE { get; set; }
        public string ONTOP_AIS_INSTALL_PLAYBOX { get; set; }
        public string ONTOP_AIS_PLAYBOX { get; set; }
        public string ONTOP_HOOQ { get; set; }
        public string ONTOP_SPECIAL_BUNDLING { get; set; }
        public string ONTOP_SPECIAL_DISCOUNT { get; set; }
        public string ONTOP_CONTENT_PLAYBOX_FREE { get; set; }
        public string ONTOP_CONTENT_PLAYBOX_SALE { get; set; }
        public string ONTOP_BOOST_SPEED { get; set; }
        public string ONTOP_WIFI_LOG_FREE { get; set; }
        public string ONTOP_WIFI_LOG_SALE { get; set; }
        public string INSTALL_FEE { get; set; }
        public string ENTRY_FEE { get; set; }
        public string PAY_IN_ADVANCE { get; set; }
        public string WIFI_ROUTER_FEE { get; set; }
        public string WIRING_FEE { get; set; }
        public string WIRING_DETAIL { get; set; }
        public string EQUIPMENT_FEE { get; set; }
        public string WIFI_ROUTER_FEE_FLAG { get; set; }
        public string DISCOUNT_WIFI_ROUTER_FEE { get; set; }
        public string ACTIVATE_PLAYBOX_FEE { get; set; }
        public string DISCOUNT_ACTIVATE_PLAYBOX_FEE { get; set; }
        public string INSTALL_FIXEDLINE_FEE { get; set; }
        public string DISCOUNT_FIXEDLINE_FEE { get; set; }
        public string TOTAL_1 { get; set; }
        public string PACKAGE_NAME { get; set; }
        public string RECURRING_CHARGE { get; set; }
        public string SPECIAL_BUNDLING_DISCOUNT { get; set; }
        public string SPECIAL_DISCOUNT { get; set; }

        public string CONTENT_PLAYBOX_NAME_SALE { get; set; }
        public string CONTENT_RECURRING { get; set; }
        public string CONTENT_PLAYBOXNAME_FREE { get; set; }
        public string CONTENT_PLAYBOX_FREE { get; set; }
        public string MA_FIXEDLINE_FEE { get; set; }
        public string ONTOP_BOOST_SPEED_NAME { get; set; }
        public string BOOST_SPEED_RECURRING_CHARGE { get; set; }
        public string MA_PLAYBOX { get; set; }
        public string TRIAL_WIFI_LOG { get; set; }
        public string WIFI_LOG { get; set; }
        public string TOTAL_2 { get; set; }
        public string TOTAL_2_DISCOUNT { get; set; }
        public string CAL_PER_DAY { get; set; }
        public string CAL_PER_DAY_DISCOUNT { get; set; }
        public string TOTAL_3 { get; set; }
        public string TOTAL_3_DISCOUNT { get; set; }

        public string ONTOP_PB_WORDDING_INSTALL { get; set; }
        public string ONTOP_PB_PRICE_INSTALL { get; set; }
        public string ONTOP_CONTENT_PB { get; set; }
        public string ONTOP_MESH { get; set; }
        public string ONTOP_MESH_CHARGE { get; set; }
        public string PLAYBOX_MONTHLY_FEE_SPOT_2 { get; set; }

        //value Existing
        public string ACTIVATE_PLAYBOX_FEE_DISCOUNT { get; set; }
        public string PLAYBOX_FEE_SPOT_2_NAME { get; set; }
        public string SPOT_2_NAME_RECURRING_CHARGE { get; set; }
        public string PLAYBOX_FEE_SPOT_2 { get; set; }
        public string PLAYBOX_FEE_SPOT_2_DISCOUNT { get; set; }
        public string SPOT_2_DISCOUNT_NAME { get; set; }
        public string PLAYBOX_FEE_SPOT_3_NAME { get; set; }
        public string SPOT_3_NAME_RECURRUNG_CHARGE { get; set; }
        public string PLAYBOX_FEE_SPOT_3 { get; set; }
        public string PLAYBOX_FEE_SPOT_3_DISCOUNT { get; set; }
        public string SPOT_3_DISCOUNT_NAME { get; set; }
        public string SPOT_1_INSTALL_NAME { get; set; }
        public string SPOT_1_INSTALL_RECURRING { get; set; }
        public string SPOT_2_INSTALL_NAME { get; set; }
        public string SPOT_2_INSTALL_RECURRING { get; set; }
        public string SPOT_3_INSTALL_NAME { get; set; }
        public string SPOT_3_INSTALL_RECURRING { get; set; }
        public string INSTALL_ADDRESS { get; set; }
        public string BILLING_ADDRESS { get; set; }
        public string SPOT_1_INSTALL_CONTENT { get; set; }
        public string SPOT_1_CONTENT_RECURRING { get; set; }

        //Mesh in web register
        public string MESH_SPOT_1 { get; set; }
        public string MESH_SPOT_2 { get; set; }
        public string MESH_SPOT_3 { get; set; }
        public string MESH_SPOT_1_DISCOUNT { get; set; }
        public string MESH_SPOT_2_DISCOUNT { get; set; }
        public string MESH_SPOT_3_DISCOUNT { get; set; }
        public string MESH_SPOT_1_DISCOUNT_NAME { get; set; }
        public string MESH_SPOT_2_DISCOUNT_NAME { get; set; }
        public string MESH_SPOT_3_DISCOUNT_NAME { get; set; }

        //Content free 2
        public string ONTOP_CONTENT_PLAYBOX_FREE_2 { get; set; }
        public string CONTENT_PLAYBOXNAME_FREE_2 { get; set; }
        public string CONTENT_PLAYBOX_FREE_2 { get; set; }

        //mesh arpu
        public string MESH_WIFI_SPOT_1_NAME { get; set; }
        public string MESH_WIFI_SPOT_1_REC { get; set; }
        public string MESH_WIFI_SPOT_2_NAME { get; set; }
        public string MESH_WIFI_SPOT_2_REC { get; set; }
        public string MESH_WIFI_SPOT_2_DISCOUNT_NAME { get; set; }
        public string MESH_WIFI_SPOT_2_DISCOUNT_REC { get; set; }
        public string MESH_WIFI_SPOT_3_NAME { get; set; }
        public string MESH_WIFI_SPOT_3_REC { get; set; }
        public string MESH_WIFI_SPOT_3_DISCOUNT_NAME { get; set; }
        public string MESH_WIFI_SPOT_3_DISCOUNT_REC { get; set; }
        //billing
        public string BILL_SUM_AVG_DAY { get; set; }
        public string BILL_SUM_AVG { get; set; }
        public string BILL_AVG_PER_DAY { get; set; }
        public string BILL_TOTAL { get; set; }
        public string BILL_SUM_TOTAL { get; set; }

        //R23.09 IP Camera
        public string ONTOP_IP_CAMERA { get; set; }
        public string IP_CAMERA_PRICE_7DAY { get; set; }
        public string IP_CAMERA_SPOT_1_DISCOUNT_NAME { get; set; }
        public string IP_CAMERA_SPOT_2_DISCOUNT_NAME { get; set; }
        public string IP_CAMERA_SPOT_3_DISCOUNT_NAME { get; set; }
        public string IP_CAMERA_SPOT_4_DISCOUNT_NAME { get; set; }
        public string IP_CAMERA_SPOT_5_DISCOUNT_NAME { get; set; }
        public string IP_CAMERA_SPOT_1 { get; set; }
        public string IP_CAMERA_SPOT_2 { get; set; }
        public string IP_CAMERA_SPOT_3 { get; set; }
        public string IP_CAMERA_SPOT_4 { get; set; }
        public string IP_CAMERA_SPOT_5 { get; set; }
        public string IP_CAMERA_SPOT_1_DISCOUNT { get; set; }
        public string IP_CAMERA_SPOT_2_DISCOUNT { get; set; }
        public string IP_CAMERA_SPOT_3_DISCOUNT { get; set; }
        public string IP_CAMERA_SPOT_4_DISCOUNT { get; set; }
        public string IP_CAMERA_SPOT_5_DISCOUNT { get; set; }
    }
}
