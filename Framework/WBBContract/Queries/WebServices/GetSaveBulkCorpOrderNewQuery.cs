using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetSaveBulkCorpOrderNewQuery : IQuery<SaveOrderResp>
    {
        private BatchBulkCorpModel _GetDetailBulkCorpRegister;
        public BatchBulkCorpModel GetBulkCorpRegister
        {
            get { return _GetDetailBulkCorpRegister ?? (_GetDetailBulkCorpRegister = new BatchBulkCorpModel()); }
            set { _GetDetailBulkCorpRegister = value; }
        }

        //  airRegistPackageRecord[] airregists;
    }
}
