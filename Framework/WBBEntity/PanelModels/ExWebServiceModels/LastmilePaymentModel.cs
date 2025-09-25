using System.ComponentModel.DataAnnotations;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class LastmilePaymentModel
    {

        [Required(ErrorMessage = "Order No is required.")]
        public string ORDER_NO { get; set; }

        [Required(ErrorMessage = "Access No is required.")]
        public string ACCESS_NO { get; set; }


        [Required(ErrorMessage = "Transaction id is required.")]
        public string TRANSACTION_ID { get; set; }


        [Required(ErrorMessage = "Order Type is required.")]
        public string ORDER_TYPE { get; set; }

        [Required(ErrorMessage = "Reused Flag is required.")]
        public string REUSED_FLAG { get; set; }

        public string REAL_DISTANCE { get; set; }
        public string MAP_DISTANCE { get; set; }
        public string DISP_DISTANCE { get; set; }
        public string ESRI_DISTANCE { get; set; }
        public string BUILDING_TYPE { get; set; }

        [Required(ErrorMessage = " User Id is required.")]
        public string USER_ID { get; set; }

        [Required(ErrorMessage = " Action Date is required.")]
        public string ACTION_DATE { get; set; }

        //R19.03
        [Required(ErrorMessage = " Request Distance is required.")]
        public string REQUEST_DISTANCE { get; set; }


        [Required(ErrorMessage = "Approve Distance  is required.")]
        public string APPROVE_DISTANCE { get; set; }

        public string APPROVE_STAFF { get; set; }

        [Required(ErrorMessage = "Approve Status  is required.")]
        public string APPROVE_STATUS { get; set; }

        //End R19.03
        public string LAST_UPDATE_BY { get; set; }
        public string LAST_UPDATE_DATE { get; set; }

        // Product Owner
        public string PRODUCT_OWNER { get; set; }

    }

    public class LastmilePaymentResponse
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESCRIPTION { get; set; }


    }

}
