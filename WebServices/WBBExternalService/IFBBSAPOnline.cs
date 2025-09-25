using System.ServiceModel;
using WBBContract.Queries.ExWebServices;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFBBSAPOnline" in both code and config file together.
    [ServiceContract]
    //[ServiceContract, XmlSerializerFormat]
    public interface IFBBSAPOnline
    {
        [OperationContract]
        NewRegistForSubmitFOAResponse NewRegistForSubmitFOA(NewRegistForSubmitFOAQuery query);

        [OperationContract]
        NewRegisterResponse NewRegistOrder(NewRegisterQuery query);

        [OperationContract]
        JoinOrderResponse JoinOrder(JoinOrderQuery query);

        [OperationContract]
        InstallationCostResponse InstallationCost(InstallationCostQuery query);

        [OperationContract]
        TerminateServiceResponse TerminateService(TerminateServiceQuery query);

        [OperationContract]
        RenewServiceResponse RenewService(RenewServiceQuery query);
    }
}
