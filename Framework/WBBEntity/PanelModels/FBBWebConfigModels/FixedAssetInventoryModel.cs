using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    //public  class FixAsset
    //{
    //       public  FixAsset()
    //          {
    //              if (fixed_asset_cur == null)
    //               fixed_asset_cur = new List<FixedAssetInventoryModel>();
    //           }
    //       public string ret_code { get; set; }

    //    //   public List<FixedAssetInventoryModel> fixed_asset_cur { get; set; }
    //}

    public class GetFixAsset
    {
        public int ret_code { get; set; }

        public List<FixedAssetInventoryModel> cur { get; set; }
    }
    public class FixedAssetInventoryModel
    {

        public string ACC_NBR { get; set; }
        public string ORD_NO { get; set; }
        public string ON_TOP1 { get; set; }
        public string ON_TOP2 { get; set; }
        public string ORDER_TYPE { get; set; }
        public string ORDER_DATE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SBC_CPY { get; set; }
        public string MATERIAL_CODE_CPESN { get; set; }
        public string CPE_SN { get; set; }

        public string MATERIAL_CODE_STBSN1 { get; set; }
        public string STB_SN1 { get; set; }
        public string MATERIAL_CODE_STBSN2 { get; set; }
        public string STB_SN2 { get; set; }
        public string MATERIAL_CODE_STBSN3 { get; set; }
        public string STB_SN3 { get; set; }


        public string MATERIAL_CODE_ATASN { get; set; }
        public string ATA_SN { get; set; }
        public string MATERIAL_CODE_WIFIROUTESN { get; set; }
        public string WIFI_ROUTER_SN { get; set; }
        public string ASSET_GI { get; set; }
        public string GI_ASSET_DATE { get; set; }
        public string ASSET_PB1 { get; set; }
        public string PB1_ASSET_DATE { get; set; }
        public string ASSET_PB2 { get; set; }
        public string PB2_ASSET_DATE { get; set; }
        public string ASSET_PB3 { get; set; }
        public string PB3_ASSET_DATE { get; set; }
        public string INSTALL_EXPENSE { get; set; }
        public string ASSET_INS { get; set; }
        public string INS_ASSET_DATE { get; set; }
        public string FLAG_TYPE { get; set; }

    }

    public class FixedAssetInventoryScreenModel
    {
        public string OrderType { get; set; }
        public string ProductName { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }


    }


}
