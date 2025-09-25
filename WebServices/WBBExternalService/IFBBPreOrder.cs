using System.ServiceModel;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFBBProfileCustomer" in both code and config file together.
    [ServiceContract]
    // ReSharper disable once InconsistentNaming
    public interface IFBBPreOrder
    {
        [OperationContract]
        string UpdateOrderStatus(string preOrderNo, string status);
    }
}
