using System.ServiceModel;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices;
using WBBContract.Queries.ExWebServices;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFBBQueryOrderForChangePack" in both code and config file together.
    [ServiceContract]
    public interface IFbbQueryOrderChangePackFieldWork
    {
        [OperationContract]
        ServiceLogNewRegisterModel QueryServiceLogNewRegister(ServiceLogNewRegisterQuery query);

        [OperationContract]
        CustomerProfileInfoModel CustomerProfileInfo(CustomerProfileInfoQuery query);

        [OperationContract]
        void CustomerRegister(ChangPackFieldworkCustomerRegisterCommand query);

        [OperationContract]
        void SaveOutgoingMessage(SaveOutgoingMessageCommand command);

        [OperationContract]
        EncryptIntraAISServiceModel TestEncryptIntraAISServiceQuery(string url);
    }


}
