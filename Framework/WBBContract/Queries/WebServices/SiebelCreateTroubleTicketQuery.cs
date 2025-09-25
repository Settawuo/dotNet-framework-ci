using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class SiebelCreateTroubleTicketQuery : IQuery<SiebelCreateTroubleTicketModel>
    {
        public string InProblemDate_End { get; set; }
        public string InMooban { get; set; }
        public string InMobileNumber { get; set; }
        public string InIndoor { get; set; }
        public string InDestModel { get; set; }
        public string InParam1 { get; set; }
        public string InFloor { get; set; }
        public string InAssetId { get; set; }
        public string InTumbol { get; set; }
        public string InRefArea { get; set; }
        public string InCurrentSignalLevel { get; set; }
        public string InStreet { get; set; }
        public string InParam2 { get; set; }
        public string InDestMobileNumber { get; set; }
        public string InAccountId { get; set; }
        public string InUsedCountry { get; set; }
        public string InChannel { get; set; }
        public string InBuilding { get; set; }
        public string InSoi { get; set; }
        public string InParam3 { get; set; }
        public string InModel { get; set; }
        public string InSubCategory { get; set; }
        public string InProvince { get; set; }
        public string InProblemDate { get; set; }
        public string InPath { get; set; }
        public string InDescription { get; set; }
        public string InOption { get; set; }
        public string InMaxSignalLevel { get; set; }
        public string InHouseNumber { get; set; }
        public string InProductName { get; set; }
        public string InDestBrand { get; set; }
        public string InAmphur { get; set; }
        public string InParam5 { get; set; }
        public string InParam4 { get; set; }
        public string InContactId { get; set; }
        public string InSymptomNote { get; set; }
        public string InOtherContactPhone { get; set; }
        public string InOperatorName { get; set; }
        public string InBrand { get; set; }
        public string InContentProvider { get; set; }
        public string InCategory { get; set; }

        public string FullURL { get; set; }
    }
}
