using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class ReservePort3bbQuery : IQuery<ReservePort3bbQueryModel>
    {
        public string splitterCode { get; set; }
        public string homeLatitude { get; set; }
        public string homeLongitude { get; set; }
        public string buildingAddressId { get; set; }
        public string transactionId { get; set; }
    }
}
