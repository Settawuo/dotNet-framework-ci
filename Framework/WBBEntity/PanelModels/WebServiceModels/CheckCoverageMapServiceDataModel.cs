using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class CheckCoverageMapServiceDataModel
    {
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public string coverage { get; set; }
        public string status { get; set; }
        public string subStatus { get; set; }
        public string addressId { get; set; }
        public string inserviceDate { get; set; }
        public List<CheckCoverageMapServiceSplitterModel> splitterList { get; set; }
        public string flowflag { get; set; }
        public string sitecode { get; set; }
    }

    public class CheckCoverageMapServiceSplitterModel
    {
        public string Name { get; set; }
        public string Lon { get; set; }
        public string Lat { get; set; }
        public string Distance { get; set; }
        public string DistanceType { get; set; }
    }

    public class CheckCoverageMapServiceConfigBody
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string transactionId { get; set; }
        public string source { get; set; }

    }

    public class CheckCoverageMapServiceConfigModel
    {
        public string Url { get; set; }
        public string ContentType { get; set; }
        public string UseSecurityProtocol { get; set; }
        public string BodyStr { get; set; }
    }

    public class CheckCoverageMapServiceConfigResult
    {
        public string returnCode { get; set; }
        public string returnMessage { get; set; }
        public string coverage { get; set; }
        public string status { get; set; }
        public string subStatus { get; set; }
        public string addressId { get; set; }
        public string inserviceDate { get; set; }
        public List<CheckCoverageMapServiceSplitterModel> splitterList { get; set; }
        public string flowflag { get; set; }
        public string sitecode { get; set; }
    }
}
