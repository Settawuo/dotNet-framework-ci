using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class GetCRMRegResultQuery : CpGateWayQueryBase
    {
        [Required]
        public string IDCardNo { get; set; }

        public string MobileNo { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public string CustFirstName { get; set; }

        [Required]
        public string CustLastName { get; set; }

        [Required]
        public string CustBirthDate { get; set; }

        [Required]
        public string ContactMobileNo { get; set; }

        public string ContactHomeNo { get; set; }

        public string ContactEmail { get; set; }

        public string PreferInstallDate { get; set; }

        public string PreferInstallTime { get; set; }

        [Required]
        public string InstallHouseNo { get; set; }

        public string InstallMoo { get; set; }

        public string InstallBuilding { get; set; }

        public string InstallTower { get; set; }

        public string InstallFloor { get; set; }

        public string InstallVillage { get; set; }

        public string InstallSoi { get; set; }

        public string InstallRoad { get; set; }

        [Required]
        public string InstallProvince { get; set; }

        [Required]
        public string InstallDistrict { get; set; }

        [Required]
        public string InstallSubDistrict { get; set; }

        [Required]
        public string InstallZipCode { get; set; }

        [Required]
        public string BillHouseNo { get; set; }

        public string BillMoo { get; set; }

        public string BillBuilding { get; set; }

        public string BillTower { get; set; }

        public string BillFloor { get; set; }

        public string BillVillage { get; set; }

        public string BillSoi { get; set; }

        public string BillRoad { get; set; }

        [Required]
        public string BillProvince { get; set; }

        [Required]
        public string BillDistrict { get; set; }

        [Required]
        public string BillSubDistrict { get; set; }

        [Required]
        public string BillZipCode { get; set; }

        //[Required]
        //public string Lat { get; set; }

        //[Required]
        //public string Long { get; set; }

        [Required]
        public List<PackageModel> SelectPackage { get; set; }

        public string LocationCode { get; set; }

        public string ASCCode { get; set; }

        public string StaffID { get; set; }

        public string SaleRep { get; set; }

        public CancelOrders CancelOrders { get; set; }
    }


    public class CancelOrders
    {
        [Required(ErrorMessage = "List Of OrderNo Can't be null")]
        public List<string> OrderNo { get; set; }
    }
}