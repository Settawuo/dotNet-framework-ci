using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class CustRegisterInfoModel
    {
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

        //public List<string> RegResult { get; set; }

        public string CvrId { get; set; }

        public string AirReturnCode { get; set; }

        public string AirReturnMessage { get; set; }

        public string AirReturnOrder { get; set; }

        public string InstallZipCodeId { get; set; }

        public string BillZipCodeId { get; set; }

        public string CoverageResultId { get; set; }

        public string SffCaNo { get; set; }

        public string SffSaNo { get; set; }

        public string SffBaNo { get; set; }

        public bool IsNonMobile { get; set; }

        public string NetworkType { get; set; }

        public string ServiceYear { get; set; }

        public string ASCCode { get; set; }

        public string StaffID { get; set; }

        public string LocationCode { get; set; }

        public string SaleRep { get; set; }

        public string DocType { get; set; }

        private RegisterResult _RegisterResult;
        public RegisterResult RegisterResult
        {
            get
            {
                return _RegisterResult ?? (_RegisterResult = new RegisterResult());
            }
            set
            {
                _RegisterResult = value;
            }
        }

        //Updates r16.8
        public string Reserved_id { get; set; }
    }

    public class RegisterResult
    {
        public string CoverageResultId { get; set; }

        public string CustomerRowId { get; set; }

        public string ReturnCode { get; set; }

        public string ReturnMessage { get; set; }

        public string ReturnIANO { get; set; }
    }
}