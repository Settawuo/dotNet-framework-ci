using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    #region DSLAM Master -- Kiakez, Advicer: P'Sam

    public class DSLAMModels
    {
        //public decimal DSLAMMODELID { get; set; }
        //public string ACTIVEFLAG { get; set; }
        //public string CREATED_BY { get; set; }
        //public System.DateTime CREATED_DATE { get; set; }
        //public string UPDATED_BY { get; set; }
        //public Nullable<System.DateTime> UPDATED_DATE { get; set; }

        [Required(ErrorMessage = "ID is required")]
        public decimal DSLAMMODELID { get; set; }

        [Required(ErrorMessage = "Model is required")]
        public string MODEL { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        public string BRAND { get; set; }

        [Required(ErrorMessage = "SH Brand is required")]
        public string SH_BRAND { get; set; }

        [Required]
        //[Required(ErrorMessage = "START INDEX is required")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please enter a value more than {1}")]
        //[Range(0,9, ErrorMessage = "Please enter number.")]
        //public decimal? SLOTSTARTINDEX { get; set; }
        public decimal SLOTSTARTINDEX { get; set; }

        [Required]
        //[Required(ErrorMessage = "MAX PORT is required")]
        //[Range(1, int.MaxValue, ErrorMessage = "Please enter a value more than {1}")]
        //[Range(1, 99, ErrorMessage = "Please enter number.")]
        public decimal MAXSLOT { get; set; }

    }

    #endregion

    #region DSLAM Master -- Boy
    public class GridDSLAMModel
    {
        public decimal DSLAMID { get; set; }
        public string Region { get; set; }
        public string RegionCode { get; set; }
        public string LotNo { get; set; }
        public decimal CurrentNo { get; set; }
        public decimal CurrentCount { get; set; }
        public decimal TotalLot { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class DSLAMMasterModel
    {
        public string DSLAMModel { get; set; }
        public string Region { get; set; }
        public string Lot { get; set; }
        public decimal DSLAMNo { get; set; }
        public string Username { get; set; }
        public List<CardViewModel> CardViewModel { get; set; }
    }

    public class CardViewModel
    {
        public string model { get; set; }
        public string cardType { get; set; }
        public string reserve { get; set; }
    }

    public class CardPortModel
    {
        public decimal CardModelID { get; set; }
        public decimal PortStartIndex { get; set; }
        public decimal MaxPort { get; set; }
        public decimal ReservePortSpare { get; set; }
    }

    public class DSLAMTempModel
    {
        public decimal DSLAMID { get; set; }
        public string Model { get; set; }
    }


    public class Card_Port
    {
        public decimal Card_Number { get; set; }
        public string Card_Model { get; set; }
        public string RESERVE_TECHNOLOGY { get; set; }
        public string Building { get; set; }
    }

    public class DSLAMMasterHistoryModel
    {
        public decimal DSLAMNUMBER { get; set; }
        public string DSLAMMODEL { get; set; }
        public string REGION_CODE { get; set; }
        public string LOT_NUMBER { get; set; }
        public string CREATED_BY { get; set; }
    }

    #endregion

}
