using System.ComponentModel.DataAnnotations;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    public class GetListImagePOIQuery
    {
        [Required]
        public string TransactionID { get; set; }

        [Required]
        public string SiteCode { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public string SubDistrict { get; set; }

        [Required]
        public string ZipCode { get; set; }
    }
}
