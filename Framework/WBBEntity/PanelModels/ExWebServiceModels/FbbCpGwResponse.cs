using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class FbbCpGwResponseBase
    {
        private string _result = "";

        public string Result
        {
            get { return _result; }
            set { _result = value; }
        }

        private string _errorCode = "";

        public string ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        private string _errorReason = "";

        public string ErrorReason
        {
            get { return _errorReason; }
            set { _errorReason = value; }
        }
    }

    public class ListPackageResponse : FbbCpGwResponseBase
    {
        public List<CpPackageModel> Packages { get; set; }
    }

    public class ListPackageByServiceResponse : FbbCpGwResponseBase
    {
        public List<PackageGroupModel> PackageModels { get; set; }
    }

    public class CheckSFFProfileResponse : FbbCpGwResponseBase
    {
        public string Province { get; set; }

        public string Amphur { get; set; }

        public string Tumbol { get; set; }

        public string PostalCode { get; set; }

        public string HouseNumber { get; set; }

        public string Mooban { get; set; }

        public string Moo { get; set; }

        public string Soi { get; set; }

        public string StreetName { get; set; }

        public string BuildingName { get; set; }

        public string Floor { get; set; }

        public string PrimaryContactFirstName { get; set; }

        public string ContactLastName { get; set; }

        public string BirthDate { get; set; }

        public string Email { get; set; }

        public string Tower { get; set; }
    }

    public class ListBuildVillResponse : FbbCpGwResponseBase
    {
        public List<ListBuildingVillageModel> BuildingVillageModelList { get; set; }
    }

    public class CheckCoverageResponse : FbbCpGwResponseBase
    {
        public List<PackageGroupModel> PackageModels { get; set; }
    }

    public class RegisterOutOfCoverageResponse : FbbCpGwResponseBase
    {
    }

    public class RegisterInCoverageResponse : FbbCpGwResponseBase
    {
        public string IANO { get; set; }
    }

    public class CheckSFFInternetProfileResponse : FbbCpGwResponseBase { }

    public class ListFBSSBuildingResponse : FbbCpGwResponseBase
    {
        public List<FBSSBuildingModel> BuildingVillageModelList { get; set; }
    }

    public class CoverageResultEnquiryResponse : FbbCpGwResponseBase { }

    public class CheckBlacklistResponse : FbbCpGwResponseBase { }

    public class ListDuplicateOrderResponse : FbbCpGwResponseBase
    {
        public List<OrderDupModel> DuplicatedOrders { get; set; }
    }

    public class GetSubContractTimeSlotResponse : FbbCpGwResponseBase
    {
        public List<FBSSTimeSlot> TimeSlots { get; set; }
    }

    public class ReserveTimeSlotResponse : FbbCpGwResponseBase { }

    public class ListImageResponse
    {
        public decimal ReturnCode { get; set; }
        public string ReturnMassage { get; set; }
        public List<PicList> PicList { get; set; }

    }
    public class PicList
    {
        public string url { get; set; }
    }

    public class CreateOrderPreRegisterResponse
    {
        public decimal ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }

    public class QueryOrderPreRegisterResponse
    {
        public decimal ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<outPreRegisterModel> PreRegisterModel { get; set; }
    }

    public class CheckPremiumFlagResponse
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public string SubAccessMode { get; set; }
        public string AccessMode { get; set; }
        public string RecurringCharge { get; set; }
        public string LocationCode { get; set; }
        public List<ReturnPremiumConfigData> ReturnPremiumConfig { get; set; }
        public List<ReturnPremiumTimeSlotData> ReturnPremiumTimeSlot { get; set; }
    }

    public class CheckTimeSlotbySubTypeResponse
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<ReturnConfigTimeslotData> ReturnConfigTimeslot { get; set; }
    }

    public class CheckFMCPackageResponse
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public string ProjectName { get; set; }
        public string OntopPackage { get; set; }
    }

    public class ReturnPremiumConfigData
    {
        public string Region { get; set; }
        public string ProvinceTh { get; set; }
        public string DistrictTh { get; set; }
        public string SubdistrictTh { get; set; }
        public string ProvinceEn { get; set; }
        public string DistrictEn { get; set; }
        public string SubdistrictEn { get; set; }
        public string Postcode { get; set; }
        public string RecurringCharge { get; set; }
        public string LocationCode { get; set; }
    }

    public class ReturnPremiumTimeSlotData
    {
        public string PartnerSubtype { get; set; }
        public string AccessMode { get; set; }
        public string NumberOfDay { get; set; }
        public string NumberOfHour { get; set; }
        public string ShiftType { get; set; }
        public string NumberOfShift { get; set; }
        public string AssignRule { get; set; }
        public string NumberOfDisplay { get; set; }
    }

    public class ReturnConfigTimeslotData
    {
        public string PartnerSubtype { get; set; }
        public string AccessMode { get; set; }
        public string NumberOfDay { get; set; }
        public string NumberOfHour { get; set; }
        public string ShiftType { get; set; }
        public string NumberOfShift { get; set; }
        public string AssignRule { get; set; }
        public string NumberOfDisplay { get; set; }
    }

    public class MicrositeWSResponse
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }

    public class MicrositeActionResponse
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }

    public class InsertCoverageRusultResponse
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }

    public class UpdateCoverageRusultResponse
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
    }


    public class PermissionUserResponse
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<DETAIL_USER_RESPONSE> USER_ARRAY { get; set; }
    }
    public class DETAIL_USER_RESPONSE
    {
        public string USER_NAME { get; set; }
    }

    public class PatchAdressESRIResponse
    {
        public string RETURN_CODE { get; set; }
        public string RETURN_MESSAGE { get; set; }
        public List<PatchAdressESRIData> list_address { get; set; }
    }

    public class QueryLOVForWebResponse
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<QueryLOVForWebData> LIST_LOV_CUR { get; set; }
    }

}