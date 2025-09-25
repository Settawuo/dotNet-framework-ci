using System;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class CardModel
    {

        public decimal CARDMODELID { get; set; }
        public string MODEL { get; set; }
        public decimal PORTSTARTINDEX { get; set; }
        public decimal MAXPORT { get; set; }
        public decimal RESERVEPORTSPARE { get; set; }
        public string DATAONLY_FLAG { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string BRAND { get; set; }

    }
}
