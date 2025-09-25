using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class CheckFlagProcessQuery : IQuery<CheckFlagProcessModel>
    {
        public string P_TYPE { get; set; }
        public string P_SUB_TYPE { get; set; }
        public string P_MOBILE_TYPE { get; set; }
        public string P_SERVICE_YEAR_BY_DAY { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string FULL_URL { get; set; }
    }
}
