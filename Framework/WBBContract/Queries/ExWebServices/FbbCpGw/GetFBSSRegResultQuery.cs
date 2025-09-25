namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    using System.ComponentModel.DataAnnotations;

    public class GetFBSSRegResultQuery : GetCRMRegResultQuery
    {
        // FBSS
        [Required]
        public string PhoneFlag { get; set; }

        [Required]
        public string TimeSlot { get; set; }

        [Required]
        public decimal TimeSlotID { get; set; }

        [Required]
        public string Guid { get; set; }

        [Required]
        public string InstallCapacity { get; set; }

        [Required]
        public string AddressId { get; set; }

        [Required]
        public string IsPartner { get; set; }

        public string PartnerName { get; set; }

        [Required]
        public string OrderRef { get; set; }
    }
}