using WBBEntity.FBBShareplexModels;

namespace WBBContract.Queries.FBBShareplex
{
    public class GetOrderByASCEmpIdDataQuery : IQuery<GetOrderByASCEmpIdModel>
    {
        public string TransactionID { get; set; }
        public string ASCCode { get; set; }
        public string EmployeeId { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string LocationCode { get; set; }
    }
}
