using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    using System.Collections.Generic;
    using WBBEntity.PanelModels;

    public enum RegResultType
    {
        CRM = 0,
        FBSS = 1,
    }

    public class GetRegResultCoreQuery : IQuery<CustRegisterInfoModel>
    {
        public RegResultType RegResultType { get; set; }

        public string TransactionID { get; set; }

        public string IDCardNo { get; set; }

        public string MobileNo { get; set; }

        public string Language { get; set; }

        public string CustFirstName { get; set; }

        public string CustLastName { get; set; }

        public string CustBirthDate { get; set; }

        public string ContactMobileNo { get; set; }

        public string ContactHomeNo { get; set; }

        public string ContactEmail { get; set; }

        public string PreferInstallDate { get; set; }

        public string PreferInstallTime { get; set; }

        public string InstallHouseNo { get; set; }

        public string InstallMoo { get; set; }

        public string InstallBuilding { get; set; }

        public string InstallTower { get; set; }

        public string InstallFloor { get; set; }

        public string InstallVillage { get; set; }

        public string InstallSoi { get; set; }

        public string InstallRoad { get; set; }

        public string InstallProvince { get; set; }

        public string InstallDistrict { get; set; }

        public string InstallSubDistrict { get; set; }

        public string InstallZipCode { get; set; }

        public string BillHouseNo { get; set; }

        public string BillMoo { get; set; }

        public string BillBuilding { get; set; }

        public string BillTower { get; set; }

        public string BillFloor { get; set; }

        public string BillVillage { get; set; }

        public string BillSoi { get; set; }

        public string BillRoad { get; set; }

        public string BillProvince { get; set; }

        public string BillDistrict { get; set; }

        public string BillSubDistrict { get; set; }

        public string BillZipCode { get; set; }

        public List<PackageModel> SelectPackage { get; set; }

        public string LocationCode { get; set; }

        public string ASCCode { get; set; }

        public string StaffID { get; set; }

        public string SaleRep { get; set; }

        public string Guid { get; set; }

        public string TimeSlotID { get; set; }

        // FBSS
        public string PhoneFlag { get; set; }

        public string TimeSlot { get; set; }

        public string InstallCapacity { get; set; }

        public string AddressId { get; set; }

        public string IsPartner { get; set; }

        public string PartnerName { get; set; }

        public string OrderRef { get; set; }

        public string LineId { get; set; }

        public string FlowFlag { get; set; }

        public string SiteCode { get; set; }

        public List<SPLITTER_INFO> Splitter { get; set; }

        public string RESERVED_ID { get; set; }
    }
}