using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    //R23.05 CheckFraud
    public class GetCustomerRiskLimitAssetWatchListQueryModel
    {
        public string resultCode { get; set; }
        public string resultDescription { get; set; }
        public string developerMessage { get; set; }
        public List<ResultData> resultData { get; set; }
    }


    public class ResultData
    {
        public LimitAsset limitAsset { get; set; }
        public WatchList watchList { get; set; }
    }

    public class LimitAsset
    {
        public string limitResult { get; set; }
        public string mobileCount { get; set; }//
        public string noOfDefaultLimit { get; set; }
        public string limitByCardNo { get; set; }
        public string limitType { get; set; }
        public string aisFlag { get; set; }
        public string statusCode { get; set; }
        public string errorMsg { get; set; }
    }

    public class WatchList
    {
        public string watchlistStatus { get; set; }
        public string watchlistCode { get; set; }
    }

    public class AthenaLimitAssetConfig
    {
        public string LOVName { get; set; }
        public string LOVVal { get; set; }
    }
}
