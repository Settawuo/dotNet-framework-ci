using System.ServiceModel;
using WBBContract.Commands.ExWebServices;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFBBQuota_SubContract" in both code and config file together.
    [ServiceContract]
    public interface IFBBQuota_SubContract
    {
        [OperationContract]
        void Quota_Subcontract(Quota_SubcontractCommand command, out int ret_code);
    }
}
