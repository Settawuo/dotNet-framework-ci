using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    //R23.05 CheckFraud
    public class GetCustomerRiskLimitAssetWatchListQuery : IQuery<GetCustomerRiskLimitAssetWatchListQueryModel>
    {
        public string Transaction_Id { get; set; }
        public string Channel { get; set; }
        public string Username { get; set; }
        [Required]
        public string IdCardNo { get; set; }
        public string LocationCode { get; set; }
        public string UserId { get; set; }
        public string CardType { get; set; }
        [Required]
        public string AssetType { get; set; }
        public string OrderType { get; set; }
        public string SmartCardFlag { get; set; }
        public string developerMessage { get; set; }
    }
}