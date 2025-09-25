using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class DelectInterfaceLogARCQuery : IQuery<DelectInterfaceLogARCModel>
    {
        // return code
        public string RET_CODE { get; set; }
        public string RET_MSG { get; set; }
    }
}
