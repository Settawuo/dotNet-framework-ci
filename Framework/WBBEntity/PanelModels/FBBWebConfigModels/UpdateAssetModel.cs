using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class UpdateAssetModel
    {

        public string Access_No { get; set; }
        public string Serial_Number { get; set; }
        public string Asset_Code { get; set; }
        public string Mat_Doc { get; set; }
        public string Doc_Year { get; set; }


    }
    public class validateDataModel
    {

        public List<UpdateAssetModel> ListUpdSuccess { get; set; }

        public List<UpdateAssetModel> ListUpdError { get; set; }
        public int UpdSuccess { get; set; }

        public int UpdUpdError { get; set; }

        public int total { get; set; }

    }


}
