namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    using System.ComponentModel.DataAnnotations;
    using WBBEntity.PanelModels.ExWebServiceModels;

    public class GetSFFInternetProfileQuery : CpGateWayQueryBase, IQuery<evOMServiceCheckChangePromotionModel>
    {
        [Required]
        public string InternetNo { get; set; }

        [Required]
        public string IDCardNo { get; set; }
    }

    public class evOMServiceCheckChangeServiceQuery : CpGateWayQueryBase, IQuery<evOMServiceCheckChangePromotionModel>
    {
        public string InternetNo { get; set; }
        public string IDCardNo { get; set; }
        public string FullUrl { get; set; }
    }
}