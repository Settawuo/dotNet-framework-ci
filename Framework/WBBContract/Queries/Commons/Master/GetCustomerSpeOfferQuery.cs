using System.Collections.Generic;
namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    public class GetCustomerSpeOfferQuery : IQuery<List<string>>
    {
        //public decimal SffChkProfLogID { get; set; }

        public string ReferenceID { get; set; }

        //public string Technology { get; set; }

        public bool IsNonMobile { get; set; }

        public string PackageGroup { get; set; }

        public bool IsAWNProduct { get; set; }
    }
}
