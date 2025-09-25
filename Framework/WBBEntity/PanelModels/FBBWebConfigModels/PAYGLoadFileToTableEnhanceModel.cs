using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class PAYGLoadFileToTableEnhanceModel
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


    public class PAYGLoadFileToTableEnhanceListResult
    {
        public int Return_Code { get; set; }
        public string Return_Desc { get; set; }
        public List<PAYGLoadFileToTableEnhanceFileList> Data { get; set; }
    }

    public class PAYGLoadFileToTableEnhanceFileList
    {
        public string file_name { get; set; }
        public string file_data { get; set; }
        public string file_index { get; set; }
    }

    public class PAYGLoadFileToTableEnhanceFileListResponse
    {
        public string filename { get; set; }
    }

}
