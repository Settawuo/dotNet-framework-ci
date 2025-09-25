using System;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    class CardModelConfigurationModel
    {
        public decimal DSLAMMODELID { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string Model { get; set; }
        public string Band { get; set; }
        public string MaxPort { get; set; }
        public int StratIndex { get; set; }
        public string Reserve { get; set; }


    }
}
