using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Commands.WebServices.FBSS
{
    public class CoverageResultEnquiryCommand : CommandBase
    {
        public string TransactionID { get; set; }

        public string OrderRef { get; set; }

        public string AddressType { get; set; }

        public string PostalCode { get; set; }

        public string SubDistrictName { get; set; }

        public string Language { get; set; }

        public string BuildingName { get; set; }

        public string BuildingNo { get; set; }

        public string PhoneFlag { get; set; }

        public string FloorNo { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string UnitNo { get; set; }

        public string Coverage { get; set; }

        public string AddressId { get; set; }

        public List<FBSSAccessModeInfo> AccessModeList { get; set; }

        public FBSSAccessModeInfo PlanningSite { get; set; }

        public string IsPartner { get; set; }

        public string PartnerName { get; set; }

        public string SplitterName { get; set; }

        public string Distance { get; set; }

        public string AddressNo { get; set; }

        public string District { get; set; }

        public string Province { get; set; }

        public string LocationCode { get; set; }

        public string AscCode { get; set; }

        public string EmployeeID { get; set; }

        public string SaleFirstname { get; set; }

        public string SaleLastname { get; set; }

        public string LocationName { get; set; }

        public string SubRegion { get; set; }

        public string Region { get; set; }

        public string AscName { get; set; }

        public string ChannelName { get; set; }

        public string SaleChannel { get; set; }

        // onservice special
        public string coverageArea { get; set; }
        public string status { get; set; }
        public string subStatus { get; set; }
        public string contactEmail { get; set; }
        public string contactTel { get; set; }
        public string groupOwner { get; set; }
        public string contactName { get; set; }
        public string networkProvider { get; set; }
        public string ftthDisplayMessage { get; set; }
        public string wttxDisplayMessage { get; set; }
    }
}