namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using WBBEntity.PanelModels.ExWebServiceModels;
    using WBBEntity.PanelModels.WebServiceModels;

    public class GetRegResultQuery : CpGateWayQueryBase, IQuery<CustRegisterInfoModel>
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

        //[Required]
        public string InstallMoo { get; set; }

        //[Required]
        public string InstallBuilding { get; set; }

        public string InstallTower { get; set; }

        public string InstallFloor { get; set; }

        //[Required]
        public string InstallVillage { get; set; }

        //[Required]
        public string InstallSoi { get; set; }

        //[Required]
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

        //[Required]
        public string BillMoo { get; set; }

        //[Required]
        public string BillBuilding { get; set; }

        public string BillTower { get; set; }

        public string BillFloor { get; set; }

        //[Required]
        public string BillVillage { get; set; }

        //[Required]
        public string BillSoi { get; set; }

        //[Required]
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

        // added new

        public string LocationCode { get; set; }

        public string ASCCode { get; set; }

        public string StaffID { get; set; }

        public string SaleRep { get; set; }

        [Required]
        public decimal TimeSlotID { get; set; }

        [Required]
        public string Guid { get; set; }

        [Required]
        public CancelOrders CancelOrders { get; set; }
    }
}