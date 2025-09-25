using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class PackageModel
    {
        public string MAPPING_CODE { get; set; }

        public string MAPPING_PRODUCT { get; set; }

        public string PACKAGE_CODE { get; set; }

        public string PACKAGE_NAME { get; set; }

        public string PACKAGE_GROUP { get; set; }

        public string PACKAGE_CLASS { get; set; }

        public decimal? RECURRING_CHARGE { get; set; }

        public string SFF_PROMOTION_BILL_THA { get; set; }

        public string SFF_PROMOTION_BILL_ENG { get; set; }

        public string TECHNOLOGY { get; set; }

        public string DOWNLOAD_SPEED { get; set; }

        public string UPLOAD_SPEED { get; set; }

        public decimal? INITIATION_CHARGE { get; set; }

        public decimal? DISCOUNT_INITIATION_CHARGE { get; set; }

        public string PACKAGE_TYPE { get; set; }

        public string PRODUCT_SUBTYPE { get; set; }

        public string PRODUCT_SUBTYPE3 { get; set; }

        public string OWNER_PRODUCT { get; set; }

        public decimal? PRE_INITIATION_CHARGE { get; set; }

        public decimal? PRE_RECURRING_CHARGE { get; set; }

        public string LABEL_SUMM_ROW1 { get; set; }

        public string LABEL_PACKAGE_ONTOP { get; set; }

        public string PACKAGE_CODE_ONTOP { get; set; }

        public string PACKAGE_NAME_ONTOP { get; set; }

        public string SFF_PROMOTION_BILL_THA_ONTOP { get; set; }

        public string SFF_PROMOTION_BILL_ENG_ONTOP { get; set; }

        public string INITIATION_CHARGE_ONTOP { get; set; }

        public string DISCOUNT_INITIATION_ONTOP { get; set; }

        public string PACKAGE_CLASS_ONTOP { get; set; }

        public string PACKAGE_CLASS_MAIN { get; set; }

        public List<SGroup> S_GROUP { get; set; }

        public List<PackageAutoModel> PACKAGE_AUTO { get; set; }

        // 17.6 Multiple Playbox
        public string PLAYBOX_EXT { get; set; }


        ///  หน้าพี่ติงลี่
        public string VOIP_IP { get; set; }

        public string IDD_FLAG { get; set; }

        public string FAX_FLAG { get; set; }

        public string MOBILE_FORWARD { get; set; }

        /// temp_ia
        public string TEMP_IA { get; set; }

        public string SelectVas_Flag { get; set; }
        public string SelectPlayBox_Flag { get; set; }
        public string SelectPlayPBL_Flag { get; set; }

        public List<SpecialOfferModel> SpecialOffer { get; set; }

        public string ACCESS_MODE { get; set; }

        public string SERVICE_CODE { get; set; }

        public string DISCOUNT_TYPE { get; set; }
        public decimal DISCOUNT_VALUE { get; set; }
        public decimal DISCOUNT_DAY { get; set; }
        public string SFF_PROMOTION_CODE { get; set; }

        // For Change Package
        public string AUTO_MAPPING_CODE { get; set; }
        public string SERENADE_FLAG { get; set; }
        public string SEQ { get; set; }

        // For ListPackage
        public string AUTO_MAPPING { get; set; }
        public string DISPLAY_FLAG { get; set; }
        public string DISPLAY_SEQ { get; set; }

        // 19.3
        public string MOBILE_PRICE { get; set; }
        public string EXISTING_MOBILE { get; set; }

        // 19.10
        public string SUB_SEQ { get; set; }

        // 20.1
        public string PACKAGE_DISPLAY_ENG { get; set; }
        public string PACKAGE_DISPLAY_THA { get; set; }
        public string PACKAGE_SERVICE_CODE { get; set; }
        public string PACKAGE_SERVICE_NAME { get; set; }
        public string PACKAGE_TYPE_DESC { get; set; }
        public decimal? PRE_PRICE_CHARGE { get; set; }
        public decimal? PRICE_CHARGE { get; set; }
        public string SFF_PRODUCT_NAME { get; set; }
        public string SFF_PRODUCT_PACKAGE { get; set; }
        public string SFF_WORD_IN_STATEMENT_ENG { get; set; }
        public string SFF_WORD_IN_STATEMENT_THA { get; set; }
        public string AUTO_MAPPING_PROMOTION_CODE { get; set; }
        //21.12PP
        public string PACKAGE_FOR_SALE_FLAG { get; set; }
        //R23.06 IP Camera
        public string PACKAGE_COUNT { get; set; }
    }

    public class PackageAutoModel
    {

        public string ACCESS_MODE { get; set; }

        public string SERVICE_CODE { get; set; }

        public string SFF_PROMOTION_CODE { get; set; }

        public string MAPPING_CODE { get; set; }

        public string MAPPING_PRODUCT { get; set; }

        public string PACKAGE_CODE { get; set; }

        public string PACKAGE_NAME { get; set; }

        public string PACKAGE_GROUP { get; set; }

        public decimal? RECURRING_CHARGE { get; set; }

        public string SFF_PROMOTION_BILL_THA { get; set; }

        public string SFF_PROMOTION_BILL_ENG { get; set; }

        public string TECHNOLOGY { get; set; }

        public string DOWNLOAD_SPEED { get; set; }

        public string UPLOAD_SPEED { get; set; }

        public decimal? INITIATION_CHARGE { get; set; }

        public decimal? DISCOUNT_INITIATION_CHARGE { get; set; }

        public string PACKAGE_TYPE { get; set; }

        public string PRODUCT_SUBTYPE { get; set; }

        public string PRODUCT_SUBTYPE3 { get; set; }

        public string OWNER_PRODUCT { get; set; }

        public decimal? PRE_INITIATION_CHARGE { get; set; }

        public decimal? PRE_RECURRING_CHARGE { get; set; }

        public string AUTO_MAPPING { get; set; }

        public string DISPLAY_FLAG { get; set; }

        public string DISPLAY_SEQ { get; set; }

        public string DISCOUNT_TYPE { get; set; }

        public decimal? DISCOUNT_VALUE { get; set; }

        public decimal? DISCOUNT_DAY { get; set; }
    }

    public class SGroup
    {
        public string PACKAGE_GROUP { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string PACKAGE_TYPE { get; set; }
    }

    public class SaveorderPackagemodel
    {
        public string TEMP_IA { get; set; }

        public string PRODUCT_SUBTYPE { get; set; }

        public string PACKAGE_TYPE { get; set; }

        public string PACKAGE_CODE { get; set; }

        public string IDD_FLAG { get; set; }

        public string FAX_FLAG { get; set; }

        public string HOME_IP { get; set; }

        public string HOME_PORT { get; set; }
    }

    public class PDFPackageModel
    {
        public string PDF_L_MAIN_PACKAGE { get; set; }

        public string PDF_L_ONTOP_ROUTER { get; set; }

        public string PDF_L_ONTOP_FIXLIND { get; set; }

        public List<string> PDF_L_ONTOPLIST { get; set; }

        /// <summary>
        /// ค่าติดตั้ง +..... ราคา
        /// </summary>
        public string PDF_L_VALUE_LABEL_TIDTUN { get; set; }

        public string PDF_L_VALUE_DETAIL_TIDTUN { get; set; }

        /// <summary>
        ///  ค่าโทรศัพท์พื่้นฐาน
        /// </summary>
        public string PDF_L_VALUE_DETAIL_BASEPHONE { get; set; }
        /// <summary>
        /// ส่วนลดค่าติดตั้งโทรศัพท์
        /// </summary>
        public string PDF_L_VALUE_DETAIL_DISCOUNTBASEPHONE { get; set; }

        // playbox ค่าติดตั้ง
        public string PDF_L_VALUE_DETAIL_PLAYBOXINSTALL { get; set; }

        // playbox ส่วนลดค่าติดตั้ง
        public string PDF_L_VALUE_DETAIL_DISCOUNTPLAYBOX { get; set; }

        // ราคาส่วนลดติดตั้ง
        public string PDF_L_VALUE_DETAIL_DISCOUNT { get; set; }

        // รวมรายการ ของชำระครั้งแรก
        public string PDF_L_VALUE_DETAIL_SUM1 { get; set; }

        //ชื่อแพ็กเกจ และราคา
        public string PDF_L_VALUE_LABEL_PAKAGENAME { get; set; }

        public string PDF_L_VALUE_DETAIL_PAKAGE { get; set; }

        /// ค่า content playbox
        public string PDF_L_VALUE_DETAIL_FULLHD { get; set; }
        public string PDF_L_VALUE_LABEL_FULLHD { get; set; }

        public string PDF_L_VALUE_DETAIL_FULLHD2 { get; set; }
        public string PDF_L_VALUE_LABEL_FULLHD2 { get; set; }

        /// ค่า speed Boost
        public string PDF_L_VALUE_DETAIL_SPEEDBOOST { get; set; }
        public string PDF_L_VALUE_LABEL_SPEEDBOOST { get; set; }

        /// <summary>
        /// ค่าบำรุงรักษาโทรศัทพ์
        /// </summary>
        public string PDF_L_VALUE_DETAIL_MAINTAINPHONE { get; set; }

        // รวมรายการชำระ ของรายเดือน
        public string PDF_L_VALUE_DETAIL_SUM2 { get; set; }

        // คิดสูงสุด4วัน
        public string PDF_L_VALUE_DETAIL_4DAY { get; set; }

        //รวมในรอบบิลแรก
        public string PDF_L_LABEL_DETAIL_ALLFIRSTBILL { get; set; }

        public string PDF_L_UNIT { get; set; }

        public string PDF_L_BUNDLING_DISCOUNT_DETAIL { get; set; }

        public string PDF_L_DISCOUNT_DETAIL { get; set; }
    }

    // GAME : newly added altenate package model.

    public class PackageGroupModel
    {
        public string OwnerProduct { get; set; }

        public string PackgaeGroup { get; set; }

        public List<PackageModel> PackageItems { get; set; }
    }

    public struct CpPackageModel
    {
        public string PACKAGE_NAME { get; set; }
    }

    public class SpecialOfferModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}