using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class evOMServiceIVRCheckBlackListQuery : IQuery<evOMServiceIVRCheckBlackListModel>
    {
        private string in_CardNo;
        public string inCardNo
        {
            get { return in_CardNo; }
            set { in_CardNo = value; }
        }

    }
}
