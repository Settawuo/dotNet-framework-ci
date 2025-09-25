using System.ComponentModel.DataAnnotations;

namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    public class ReserveTimeSlotQuery : CpGateWayQueryBase
    {
        [Required]
        public string Guid { get; set; }

        [Required]
        public decimal TimeSlotID { get; set; }

        [Required]
        public string ReserveDateTime { get; set; }
    }
}
