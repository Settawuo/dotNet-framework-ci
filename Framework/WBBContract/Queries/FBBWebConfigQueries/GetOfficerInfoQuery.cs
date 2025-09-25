using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigModels
{
    public class GetOfficerInfoQuery : IQuery<GetEmployeeProfileByPINModel>
    {
        public string EMP_CODE { get; set; }
    }
}
