using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    //R23.05 CheckFraud
    public class GetOnlineQueryCheckFraudQueryModel
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
        public string TRANSACTION_ID { get; set; }
        public checkFraudInfo CHECK_FRAUD_INFO { get; set; }
    }

    public class checkFraudInfo
    {
        public string NOTIFY_POPUP { get; set; }
        public string NOTIFY_MESSAGE { get; set; }
        public string FLAG_GO_NOGO { get; set; }
        public string VERIFY_REASON { get; set; }
        public string AUTO_CREATE_PROSPECT { get; set; }
        public string FRAUD_SCORE { get; set; }
        public List<FRAUDREASONS> FRAUD_REASONS { get; set; }
        public string CEN_FRAUD_FLAG { get; set; }

    }

    public class FRAUDREASONS
    {
        public string REASON { get; set; }
        public decimal? SCORE { get; set; }
    }
}
