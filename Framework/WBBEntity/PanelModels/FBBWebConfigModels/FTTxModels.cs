using System;
using System.ComponentModel;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class FTTxModels
    {
    }

    public class GridFTTxModel
    {
        public string RegionCode { get; set; }
        public string Province { get; set; }
        public string Amphur { get; set; }
        public string Tumbon { get; set; }

        public string OwnerProduct { get; set; }
        public string OwnerType { get; set; }
        public decimal FTTX_ID { get; set; }
        public string tower_th { get; set; }
        public string tower_en { get; set; }
        public string Coverage_Status { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }

        public DateTime? ONTARGET_DATE_IN { get; set; }
        public DateTime? ONTARGET_DATE_EX { get; set; }
        //public DateTime? ONTARGET_DATE { get; set; }
        public string SERVICE_TYPE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string GroupAmphur { get; set; }

        public string RegionCodeEN { get; set; }
        public string ProvinceEN { get; set; }
        public string AmphurEN { get; set; }
        public string TumbonEN { get; set; }
        public string Zipcode { get; set; }

    }


    public class GridFTTxFindGroupAmper
    {
        public string RegionCode { get; set; }
        public string Province { get; set; }
        public string Amphur { get; set; }
        public string GroupAmphur { get; set; }

    }

    public class GridFTTxExcelModel
    {
        public string RegionCode { get; set; }
        public string Province { get; set; }
        public string Amphur { get; set; }
        public string Tumbon { get; set; }
        public string OwnerProduct { get; set; }
        public string SERVICE_TYPE { get; set; }
        public string OwnerType { get; set; }
        public string tower_th { get; set; }
        public string tower_en { get; set; }
        public string ONTARGET_DATE_IN { get; set; }
        public string ONTARGET_DATE_EX { get; set; }
    }

    public class CountCoverageModel
    {
        public decimal Total { get; set; }
        public decimal NSN { get; set; }
        public decimal SIMAT { get; set; }
        public decimal AIS { get; set; }
        public decimal SYMPHONY { get; set; }
    }

    public class FTTxHistoryModel
    {
        [DisplayName("Owner Product")]
        public string OWNER_PRODUCT { get; set; }
        [DisplayName("Owner Type")]
        public string OWNER_TYPE { get; set; }
        [DisplayName("Province")]
        public string PROVINCE { get; set; }
        [DisplayName("District")]
        public string AMPHUR { get; set; }
        [DisplayName("Group Amphur")]
        public string GroupAmphur { get; set; }
        [DisplayName("Tower Thai")]
        public string tower_th { get; set; }
        [DisplayName("Tower English")]
        public string tower_en { get; set; }

        [DisplayName("Service Type")]
        public string Service_Type { get; set; }
        //[DisplayName("OnTarget_date")]
        //public DateTime ? OnTarget_date { get; set; }

    }

}
