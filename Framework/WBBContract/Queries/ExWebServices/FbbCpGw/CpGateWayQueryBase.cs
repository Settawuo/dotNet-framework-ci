namespace WBBContract.Queries.ExWebServices.FbbCpGw
{
    using System.ComponentModel.DataAnnotations;
    public class CpGateWayQueryBase
    {
        [Required]
        public string TransactionID { get; set; }
    }
}