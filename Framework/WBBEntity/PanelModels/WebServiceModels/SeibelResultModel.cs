using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{
    public class SeibelResultModel
    {
        public string outASCPartnerName { get; set; }
        public string outBusinessType { get; set; }
        public string outCharacteristic { get; set; }
        public string outErrorMessage { get; set; }
        public string outFullAddress { get; set; }
        public string outLocationCode { get; set; }
        public string outMainFax { get; set; }
        public string outMainPhone { get; set; }
        public string outMobileNo { get; set; }
        public string outOperatorClass { get; set; }

        public string outPartnerName { get; set; }
        public string outProvince { get; set; }
        public string outRegion { get; set; }
        public string outStatus { get; set; }
        public string outSubRegion { get; set; }
        public string outSubType { get; set; }
        public string outTitle { get; set; }
        public string outType { get; set; }
        public string outWTName { get; set; }
        public string outLocationEmailByRegion { get; set; }
        public string outMemberCategory { get; set; }
        //20.3
        public string outCompanyName { get; set; }
        public string outDistChn { get; set; }
        public string outChnSales { get; set; }
        public string outShopType { get; set; }
        public string outASCTitleThai { get; set; }
        public string outPosition { get; set; }
        public string outLocationName { get; set; }

        //R21.5 Pool Villa
        public string outLocationProvince { get; set; }
        public List<SeibelAddressLocation> addressLocationList { get; set; }
        public string outChnSalesCode { get; set; }
    }

    public class SeibelAddressLocation
    {
        public string outAddressID { get; set; }
        public string outAddressType { get; set; }
        public string outHouseNo { get; set; }
        public string outMoo { get; set; }
        public string outMooban { get; set; }
        public string outBuilding { get; set; }
        public string outFloor { get; set; }
        public string outRoom { get; set; }
        public string outSoi { get; set; }
        public string outStreet { get; set; }
        public string outProvince { get; set; }
        public string outAmphur { get; set; }
        public string outTumbol { get; set; }
        public string outZipcode { get; set; }
        public string outCountry { get; set; }
        public string outFullAddress { get; set; }
        public string outAddressLastUpd { get; set; }
    }
}
