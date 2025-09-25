using System.ServiceModel;
using WBBContract.Commands.ExWebServices.FbbPaygLMD;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFBBPAYG_LMD_Interface" in both code and config file together.
    [ServiceContract]
    public interface IFBBPAYG_LMD_Interface
    {
        [OperationContract]
        UpdateInvoiceResponse UpdateInvoice(UpdateInvoice command);

        [OperationContract]
        UpdateOrderStatusResponse UpdateOrderStatus(UpdateOrderStatus command);

    }
}
