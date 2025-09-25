using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetOnlineQueryMobileInfoQuery : IQuery<GetOnlineQueryMobileInfoModel>
    {
        public string FullUrl { get; set; }
        public string Internet_No { get; set; }
        public string Transaction_Id { get; set; }
        public string Request_Url { get; set; }

        public OnlineQueryMobileInfoBody Body { get; set; }
        public string BodyJson { get; set; }
    }

    public class OnlineQueryMobileInfoBody
    {
        public string MOBILE_NO { get; set; }
        public string FIBRE_ID { get; set; }
        public string ID_CARD { get; set; }
        public string ID_CARD_TYPE { get; set; }
        public string LOCATION_CODE { get; set; }
    }

    public class GetOnlineQueryPackPenaltyQuery : IQuery<GetOnlineQueryPackPenaltyModel>
    {
        public string FullUrl { get; set; }
        public string Internet_No { get; set; }
        public string Transaction_Id { get; set; }
        public string Request_Url { get; set; }

        public GetOnlineQueryPackPenaltyBody Body { get; set; }
        public string BodyJson { get; set; }
    }

    public class GetOnlineQueryPackPenaltyBody
    {
        public string SFF_PROMOTION_CODE { get; set; }
        public string PRODUCT_SUBTYPE { get; set; }
        public string OWNER_PRODUCT { get; set; }
        public string SALE_CHANNEL { get; set; }
    }
}
