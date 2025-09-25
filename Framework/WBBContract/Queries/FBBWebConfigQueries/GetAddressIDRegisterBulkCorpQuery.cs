using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetAddressIDRegisterBulkCorpQuery : IQuery<RetGetAddrID>
    {
        public string P_EVENT_CODE { get; set; }
        public string P_ADDRESS_ID { get; set; }
    }
}
