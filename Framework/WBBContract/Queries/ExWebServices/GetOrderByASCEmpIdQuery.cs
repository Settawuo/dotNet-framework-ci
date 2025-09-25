namespace WBBContract.Queries.ExWebServices
{
    public class GetOrderByASCEmpIdQuery
    {
        public string TransactionID { get; set; }
        public string ASCCode { get; set; }
        public string EmployeeId { get; set; }
        public string LocationCode { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
