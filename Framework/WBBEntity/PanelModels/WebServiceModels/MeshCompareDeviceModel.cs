using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class MeshCompareDeviceModel
    {
        public MeshCompareDeviceModel()
        {
            if (RES_COMPLETE_CUR == null)
                RES_COMPLETE_CUR = new List<CompareDevice>();
        }
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<CompareDevice> RES_COMPLETE_CUR { get; set; }
    }

    public class CompareDevice
    {
        public string FIBRENETID { get; set; }
        public string MESH_BRAND_NAME { get; set; }
        public string MESH_PROMOTION_CODE { get; set; }
        public string CHANNEL_MESH { get; set; }
        public string NUMBER_OF_PURCHSES { get; set; }
        public string FLAG_POPUP_MESH { get; set; }
        public string DETAIL_MESH { get; set; }
        public byte[] PIC_MESH { get; set; }
        public string PIC_MESH_BASE64 { get; set; }
        public string OPTION_MESH { get; set; }
        public string OPTION_MESH_DETAIL { get; set; }
        public string POPUP_ALERT { get; set; }
        public string PRICE_VALUE { get; set; }
        public string WORDING_DISPLAY { get; set; }
        public string ORDER_BY { get; set; }
        public int? ODR { get; set; }
        public string CONTRACT_ID { get; set; }
        public string CONTRACT_NAME { get; set; }
        public string DURATION { get; set; }
        //R22.05 Device Contract Mesh
        public string PROMOTION_DEVICE { get; set; }
        //R22.11 Mesh with arpu
        public string FLAG_OPTION { get; set; }
        public string FLAG_MESH { get; set; }
    }
}
