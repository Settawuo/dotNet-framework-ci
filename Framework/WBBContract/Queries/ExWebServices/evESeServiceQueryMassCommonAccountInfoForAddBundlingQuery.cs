using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class evESeServiceQueryMassCommonAccountInfoForAddBundlingQuery :
        IGetMassCommonAccount, IQuery<evESeServiceQueryMassCommonAccountInfoModel>
    {
        private string _ReferenceID;
        public string ReferenceID
        {
            get { return _ReferenceID; }
            set { _ReferenceID = value; }
        }

        private string in_Option;
        public string inOption
        {
            get { return in_Option; }
            set { in_Option = value; }
        }

        private string in_MobileNo;
        public string inMobileNo
        {
            get { return in_MobileNo; }
            set { in_MobileNo = value; }
        }

        private string in_CardNo;
        public string inCardNo
        {
            get { return in_CardNo; }
            set { in_CardNo = value; }
        }

        private string in_CardType;
        public string inCardType
        {
            get { return in_CardType; }
            set { in_CardType = value; }
        }

        private string page;
        public string Page
        {
            get { return page; }
            set { page = value; }
        }

        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        // Update 17.5
        public string FullUrl { get; set; }
    }
}
