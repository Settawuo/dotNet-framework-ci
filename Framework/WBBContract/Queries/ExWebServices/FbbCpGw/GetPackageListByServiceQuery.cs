using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    public class GetPackageListByServiceQuery
    {
        [Required]
        public string TransactionID { get; set; }

        [Required]
        public List<string> ListAccessMode { get; set; }

        [Required]
        public string IsPartner { get; set; }

        [Required]
        public string PartnerName { get; set; }
    }
}