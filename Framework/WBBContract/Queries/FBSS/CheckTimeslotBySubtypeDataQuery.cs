using WBBEntity.FBSSModels;

namespace WBBContract.Queries.FBSS
{
    public class CheckTimeslotBySubtypeDataQuery : IQuery<CheckTimeslotBySubtypeModel>
    {
        public string partnersubtype { get; set; }
        public string accessmode { get; set; }
        public string TransactionID { get; set; }
    }
}
