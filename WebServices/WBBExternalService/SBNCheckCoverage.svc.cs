using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBExternalService
{
    public class SBNCheckCoverage : ISBNCheckCoverage
    {
        private ILogger _logger;
        private IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;

        public SBNCheckCoverage(ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<InterfaceLogCommand> intfLogCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _intfLogCommand = intfLogCommand;
        }

        public SBNCheckCoverageResponse PortAvaliable(GetPortAvaliableQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public SBNCheckCoverageResponse AssignNewPort(GetAssignNewPortQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        /// <summary>
        /// Obsoleted Use ChangePort Instead.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public SBNCheckCoverageResponse ChangePortFail(GetChangePortFailQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public SBNCheckCoverageResponse ChangePort(GetChangePortQuery query)
        {
            InterfaceLogCommand log = null;

            log = StartInterface<GetChangePortQuery>(query, "ChangePort");

            var response = _queryProcessor.Execute(query);

            EndInterface<SBNCheckCoverageResponse>(response, log,
                response.RETURN_CODE.ToSafeString(), response.RETURN_DESC);

            return response;
        }

        public SBNCheckCoverageResponse ActivePort(GetActivePortQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public SBNCheckCoverageResponse CoverageRegion(GetCoverageRegionQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public SBNCheckCoverageResponse AirnetWirelessCoverage(GetAirnetWirelessCoverageQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        // new 23/04/2014
        public SBNCheckCoverageResponse ReAvailablePort(GetReAvailableQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        // new 07/05/2014
        public SBNCheckCoverageResponse AvaliablePort(GetAvaliablePortQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public void Available_VOIP_IP(string IP, string REFF_USER, out int RETURN_CODE, out string RETURN_DESC)
        {
            //var Available_VOIP_IP_Para = new AvailableVOIPIPCommand()
            //{
            //    IP_ADDRESS = IP,
            //    REFF_USER = REFF_USER
            //};

            ///_AvailableVOIPIPCommand.Handle(Available_VOIP_IP_Para);

            RETURN_CODE = 0;
            RETURN_DESC = "";
        }

        public void Assign_VOIP_IP(string REFF_USER, string REFF_KEY, out int RETURN_CODE, out string RETURN_DESC, out AssignVOIPIPCommand SBNResponse)
        {

            //var command = new AssignVOIPIPCommand()
            //{
            //    REFF_USER = REFF_USER,
            //    REFF_KEY = REFF_KEY
            //};

            RETURN_CODE = 0;
            RETURN_DESC = "";
            SBNResponse = null;

        }

        private InterfaceLogCommand StartInterface<T>(T query, string methodName)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                //IN_TRANSACTION_ID = "",
                METHOD_NAME = methodName,
                SERVICE_NAME = query.GetType().Name,
                //IN_ID_CARD_NO = "",
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "AIRNETWF",
                CREATED_BY = "FBBAIR",
            };

            _intfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        private void EndInterface<T>(T output, InterfaceLogCommand dbIntfCmd, string result, string reason)
        {
            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = (result == "Success") ? output.DumpToXml() : reason;

            _intfLogCommand.Handle(dbIntfCmd);
        }
    }
}