using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetConfirmChangePromotionQuery : IQuery<ConfirmChangePromotionModelLine4>
    {
        public string orderType { get; set; }
        public string mobileNo { get; set; }
        public string orderReson { get; set; }
        public string orderChannel { get; set; }
        public string userName { get; set; }
        public string chargeFeeFlag { get; set; }
        public string promotionCode { get; set; }
        public string actionStatus { get; set; }
        public string FlagCallService_evOMServiceConfirmChangePromotion { get; set; }
        public string FlagCallService_evOMCreateOrderChangePromotion { get; set; }
        public string promotionCdOldContent { get; set; }

        //R18.4
        public string locationCd { get; set; }
        public string ascCode { get; set; }
        public string employeeID { get; set; }
        public string employeeName { get; set; }
    }

    public class CheckNewRegisProspectQuery : IQuery<CheckNewRegisProspectQueryModel>
    {
        public string idCardNo { get; set; }
        public string locationCd { get; set; }
        public string ascCd { get; set; }
        public string TRANSACTION_ID { get; set; }
    }

}
