using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices.FBSS
{
    public class GetFBSSFeasibilityCheck : IQuery<FBSSCoverageResult>
    {
        public string AddressType { get; set; }
        public string PostalCode { get; set; }
        public string Language { get; set; }
        public string SubDistricName { get; set; }
        public string BuildingName { get; set; }
        public string BuildingNo { get; set; }
        public string PhoneFlag { get; set; }
        public string FloorNo { get; set; }
        public string Latitude { get; set; } // changed
        public string Longitude { get; set; } // changed
        public string UnitNo { get; set; }

        public decimal InterfaceLogId { get; set; }

        // Update 17.2
        public string TransactionId { get; set; }

        // Update 17.5
        public string FullUrl { get; set; }
    }
}
