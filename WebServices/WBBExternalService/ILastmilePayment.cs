using System.ServiceModel;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ILastmilePayment" in both code and config file together.
    [ServiceContract]
    public interface ILastmilePayment
    {
        // [OperationContract]
        // [OperationContract]
        //  LastmilePaymentResponse LastmilePaymentResponseWcf(LastmilePaymentQuery query);
        //   [OperationContract]
        // string UpdateOrderStatus(string preOrderNo, string status);
        //[OperationContract]
        //void DoWork();
        // [OperationContract]
        [OperationContract]
        LastmilePaymentResponse LastmilePaymentResponseWcf(LastmilePaymentModel model);
        //   [OperationContract]
        // string UpdateOrderStatus(string preOrderNo, string status);
        //[OperationContract]

    }
}
