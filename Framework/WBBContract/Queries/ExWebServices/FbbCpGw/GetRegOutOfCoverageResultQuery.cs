using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    public class GetRegOutOfCoverageResultQuery : CpGateWayQueryBase, IQuery<List<string>>
    {
        public string IDCardNo { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public string CustName { get; set; }

        [Required]
        public string ContactMobileNo { get; set; }

        //[Required]
        //public string OrderRef { get; set; }

        public string EmailAddress { get; set; }
        public string LineId { get; set; }

        public string AddressTypeDTL { get; set; }
        public string Remark { get; set; }
        public string Technology { get; set; }
        public string Projectname { get; set; }

        // onservice special
        //public string status { get; set; }
        //public string subStatus { get; set; }
        //public string contactEmail { get; set; }
        //public string contactTel { get; set; }
        //public string groupOwner { get; set; }
        //public string contactName { get; set; }
        //public string networkProvider { get; set; }
        //public string ftthDisplayMessage { get; set; }
        //public string wttxDisplayMessage { get; set; }
    }
}