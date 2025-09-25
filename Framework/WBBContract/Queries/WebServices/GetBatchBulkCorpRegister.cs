using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetBatchBulkCorpRegister : IQuery<GetBlukCorpRegisterModel>
    {
        public string p_test_parameter { get; set; }
    }

}
