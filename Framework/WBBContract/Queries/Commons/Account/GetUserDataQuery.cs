using WBBEntity.PanelModels.Account;

namespace WBBContract.Queries.Commons.Account
{
    public class GetUserDataQuery : IQuery<UserModel>
    {
        public string UserName { get; set; }
        public string AuthenType { get; set; }
        public string PinCode { get; set; }
        public string GroupId { get; set; }
    }
}
