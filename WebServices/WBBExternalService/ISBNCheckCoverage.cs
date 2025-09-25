using System.ServiceModel;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBExternalService
{
    [ServiceContract]
    public interface ISBNCheckCoverage
    {
        [OperationContract]
        SBNCheckCoverageResponse PortAvaliable(GetPortAvaliableQuery query);

        [OperationContract]
        SBNCheckCoverageResponse AssignNewPort(GetAssignNewPortQuery query);

        [OperationContract]
        SBNCheckCoverageResponse ChangePortFail(GetChangePortFailQuery query);

        [OperationContract]
        SBNCheckCoverageResponse ChangePort(GetChangePortQuery query);

        [OperationContract]
        SBNCheckCoverageResponse AirnetWirelessCoverage(GetAirnetWirelessCoverageQuery query);

        [OperationContract]
        SBNCheckCoverageResponse ActivePort(GetActivePortQuery query);

        [OperationContract]
        SBNCheckCoverageResponse AvaliablePort(GetAvaliablePortQuery query);

        //New 07102557
        [OperationContract]
        void Available_VOIP_IP(string IP, string REFF_USER, out int RETURN_CODE, out string RETURN_DESC);
        [OperationContract]
        void Assign_VOIP_IP(string REFF_USER, string REFF_KEY, out int RETURN_CODE, out string RETURN_DESC, out AssignVOIPIPCommand SBNResponse);
    }
}