using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class evESeServiceQueryPrivilegeByMobileNoQuery : IQuery<evESeServiceQueryPrivilegeByMobileNoModel>
    {
        public string mobileNo { get; set; }
    }
}
