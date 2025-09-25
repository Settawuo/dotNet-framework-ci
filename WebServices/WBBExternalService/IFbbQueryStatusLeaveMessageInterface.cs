using System.ServiceModel;
using WBBContract.Queries.ExWebServices;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFbbQueryStatusLeaveMessageInterface" in both code and config file together.
    [ServiceContract]
    public interface IFbbQueryStatusLeaveMessageInterface
    {
        [OperationContract]
        ListStatusLeaveMessageResponse LeaveMessageQuery(GetListStatusLeaveMessageQuery query);
    }
}
