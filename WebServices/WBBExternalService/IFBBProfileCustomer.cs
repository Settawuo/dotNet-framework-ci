using System.ServiceModel;
using WBBContract.Commands;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFBBProfileCustomer" in both code and config file together.
    [ServiceContract]
    public interface IFBBProfileCustomer
    {
        [OperationContract]
        void SaveProfileCustomer(ProfileCustomerCommand command, out int ret_code, out string ret_message);
    }




}
