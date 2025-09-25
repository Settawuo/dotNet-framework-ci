using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class RespondGetDataForSapFOAWriteOff
    {
        [DataMember(Order = 1)]
        public string DOC_DATE { get; set; }

        [DataMember(Order = 2)]
        public string POST_DATE { get; set; }

        [DataMember(Order = 3)]
        public string TRANS_ID { get; set; }

        [DataMember(Order = 4)]
        public string REF_DOC { get; set; }

        [DataMember(Order = 5)]
        public string RUN_GROUP { get; set; }

        [DataMember(Order = 6)]
        public string MOVEMENT_TYPE { get; set; }

        [DataMember(Order = 7)]
        public string MATERIAL_NO { get; set; }

        [DataMember(Order = 8)]
        public string PLANT_FROM { get; set; }

        [DataMember(Order = 9)]
        public string SLOC_FROM { get; set; }

        [DataMember(Order = 10)]
        public string PLANT_TO { get; set; }

        [DataMember(Order = 11)]
        public string SLOC_TO { get; set; }

        [DataMember(Order = 12)]
        public string QUANTITY { get; set; }

        [DataMember(Order = 13)]
        public string UOM { get; set; }

        [DataMember(Order = 14)]
        public string COST_CENTER { get; set; }

        [DataMember(Order = 15)]
        public string GL_ACCT { get; set; }

        [DataMember(Order = 16)]
        public string GOODS_RECIPIENT { get; set; }

        [DataMember(Order = 17)]
        public string MATERIAL_DOC { get; set; }

        [DataMember(Order = 18)]
        public string DOC_YEAR { get; set; }

        [DataMember(Order = 19)]
        public string ITEM_TEXT { get; set; }

        [DataMember(Order = 20)]
        public string SERIAL_NO { get; set; }

        [DataMember(Order = 21)]
        public string REF_DOC_FBSS { get; set; }

        [DataMember(Order = 22)]
        public string XREF1_HD { get; set; }

    }
    public class SearchFoaWriteOffModel
    {
        public string ACCESS_NO { get; set; }
        public string SERIAL_NO { get; set; }

    }
    public class FBSSFixedAssetSnAct
    {
        public string ACCESS_NUMBER { get; set; }
        public string SN { get; set; }
        public string COMPANY_CODE { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        public decimal SERVICE_SEQ { get; set; }
        public string SNPATTERN { get; set; }
        public string SN_STATUS { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string ASSET_CODE { get; set; }
        public string MATERIAL_DOC { get; set; }
        public string DOC_YEAR { get; set; }
        public string MATERIAL_DOC_MT { get; set; }
        public string DOC_YEAR_MT { get; set; }
        public string MATERIAL_DOC_RET { get; set; }
        public string DOC_YEAR_RET { get; set; }


    }
    public class ReturnCallSapWriteOffModel
    {

        public List<RespondGetDataForSapFOAWriteOff> p_ws_writeoff_cur { get; set; }

        public string ret_code { get; set; }
        public string ret_msg { get; set; }


        public int ResSuccess { get; set; }
        public int ResError { get; set; }


    }
    public static class TempDataTableWriteOff
    {
        public static DataTable dt { get; set; }

    }
    public class FoaWriteOffModel
    {
        public string ACCESS_NUMBER { get; set; }
        public string ORDER_TYPE { get; set; }
        public string SN { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PLANT { get; set; }
        public string STORAGE_LOCATION { get; set; }
        public string MOVEMENT_TYPE { get; set; }
        //public string SNPATTERN { get; set; }
        //public string SN_STATUS { get; set; }
        public string CREATE_BY { get; set; }
    }
    public class FoaWriteOffResponse
    {
        public string result { get; set; }
        public string errorReason { get; set; }
    }
}
