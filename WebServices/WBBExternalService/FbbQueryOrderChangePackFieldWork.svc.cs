using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices;
using WBBContract.Queries.ExWebServices;
using WBBContract.Queries.WebServices;
using WBBEntity.PanelModels.ExWebServiceModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBExternalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FbbQueryOrderChangePackFieldWork" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select FbbQueryOrderChangePackFieldWork.svc or FbbQueryOrderChangePackFieldWork.svc.cs at the Solution Explorer and start debugging.
    public class FbbQueryOrderChangePackFieldWork : IFbbQueryOrderChangePackFieldWork
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<ChangPackFieldworkCustomerRegisterCommand> _commandHandler;
        private readonly ICommandHandler<SaveOutgoingMessageCommand> _commandSaveOutgoingHandler;

        public FbbQueryOrderChangePackFieldWork(IQueryProcessor queryProcessor, ICommandHandler<ChangPackFieldworkCustomerRegisterCommand> commandHandler, ICommandHandler<SaveOutgoingMessageCommand> commandSaveOutgoingHandler)
        {
            _queryProcessor = queryProcessor;
            _commandHandler = commandHandler;
            _commandSaveOutgoingHandler = commandSaveOutgoingHandler;
        }

        public ServiceLogNewRegisterModel QueryServiceLogNewRegister(ServiceLogNewRegisterQuery query)
        {
            if (string.IsNullOrEmpty(query.OrderNo))
                return new ServiceLogNewRegisterModel { Return_Message = "[OrderNo] cannot be null." };

            if (string.IsNullOrEmpty(query.ServiceName))
                return new ServiceLogNewRegisterModel { Return_Message = "[ServiceName] cannot be null." };

            return _queryProcessor.Execute(query);
        }

        public CustomerProfileInfoModel CustomerProfileInfo(CustomerProfileInfoQuery query)
        {
            if (string.IsNullOrEmpty(query.InternetNo))
                return new CustomerProfileInfoModel { Return_Message = "[InternetNo] cannot be null." };

            return _queryProcessor.Execute(query);
        }


        public void CustomerRegister(ChangPackFieldworkCustomerRegisterCommand command)
        {
            _commandHandler.Handle(command);
        }

        public void SaveOutgoingMessage(SaveOutgoingMessageCommand command)
        {
            _commandSaveOutgoingHandler.Handle(command);
        }

        public EncryptIntraAISServiceModel TestEncryptIntraAISServiceQuery(string url)
        {
            var query = new EncryptIntraAISServiceQuery()
            {
                Url = "https://10.137.16.115:8443",
                p_transaction_id = "T2024611244928136737",
                //p_non_mobile_no = "8850002253",
                body = new EncryptIntraAISServiceBody
                {
                    ssid = "T2024611244928136737",
                    command = "encrypt",
                    input = "8850002253 31500050073539 W-TS-1047-6309-0000044"
                }
            };
            if (!string.IsNullOrEmpty(url))
            {
                query.Url = url;
            }
            return _queryProcessor.Execute(query);
        }

    }
}
