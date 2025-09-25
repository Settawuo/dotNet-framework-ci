using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.WebServices
{
    // R23.05.2023 Created: THOTST49
    public class InsertConsentLogNewRegisterCommandHandle : ICommandHandler<InsertConsentLogNewRegisterCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<string> _objService;
        private readonly IWBBUnitOfWork _uow;

        public InsertConsentLogNewRegisterCommandHandle(ILogger logger, IWBBUnitOfWork unitOfWork, IEntityRepository<FBB_INTERFACE_LOG> entityRepository, IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = unitOfWork;
            _intfLog = entityRepository;
            _objService = objService;
        }

        public void Handle(InsertConsentLogNewRegisterCommand command)
        {
            InterfaceLogCommand log = null;

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.ContactMobile, "InsertConsentLogNewRegisterCommandHandle", "InsertConsentLogNewRegisterCommand", command.ContactMobile, "FBB", "WEB");

                var P_CREATED_BY = new OracleParameter()
                {
                    ParameterName = "P_CREATED_BY",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = "WBB"
                };

                var P_UPDATED_BY = new OracleParameter()
                {
                    ParameterName = "P_UPDATED_BY",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = "WBB"
                };

                var P_INTERNET_NO = new OracleParameter()
                {
                    ParameterName = "P_INTERNET_NO",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = command.InTransactionId
                };

                var P_CHANNEL = new OracleParameter()
                {
                    ParameterName = "P_CHANNEL",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = "CUSTOMER"
                };

                var P_LOCATION_CODE = new OracleParameter()
                {
                    ParameterName = "P_LOCATION_CODE",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = DBNull.Value
                };

                var P_ASC_CODE = new OracleParameter()
                {
                    ParameterName = "P_ASC_CODE",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = DBNull.Value
                };

                var P_EMPLOYEE_ID = new OracleParameter()
                {
                    ParameterName = "P_EMPLOYEE_ID",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = DBNull.Value
                };

                var P_EMPLOYEE_NAME = new OracleParameter()
                {
                    ParameterName = "P_EMPLOYEE_NAME",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = DBNull.Value
                };

                var P_CONTACT_MOBILE = new OracleParameter()
                {
                    ParameterName = "P_CONTACT_MOBILE",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = command.ContactMobile
                };

                var P_TYPE_FLAG = new OracleParameter()
                {
                    ParameterName = "P_TYPE_FLAG",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = "NEW_REGISTER_CONSENT_FLAG"
                };

                var P_VALUE_FLAG = new OracleParameter()
                {
                    ParameterName = "P_VALUE_FLAG",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = command.ValueFlag ? "Y" : "N"
                };

                var P_REF_ORDER_NO = new OracleParameter()
                {
                    ParameterName = "P_REF_ORDER_NO",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = command.RefOrderNo
                };

                var P_REMARK = new OracleParameter()
                {
                    ParameterName = "P_REMARK",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = JsonConvert.SerializeObject(command)
                };

                var P_CLIENT_ID = new OracleParameter()
                {
                    ParameterName = "P_CLIENT_ID",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = command.ClientIp
                };

                var RETURN_CODE = new OracleParameter()
                {
                    ParameterName = "RETURN_CODE",
                    OracleDbType = OracleDbType.Int32,
                    Direction = ParameterDirection.Output
                };

                var RETURN_MESSAGE = new OracleParameter()
                {
                    ParameterName = "RETURN_MESSAGE",
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 32767,
                    Direction = ParameterDirection.Output
                };

                var executeResult = _objService.ExecuteStoredProc("wbb.PKG_FBB_INSERT_LOG.INSERT_CONSENT_LOG", new
                {
                    P_CREATED_BY,
                    P_UPDATED_BY,
                    P_INTERNET_NO,
                    P_CHANNEL,
                    P_LOCATION_CODE,
                    P_ASC_CODE,
                    P_EMPLOYEE_ID,
                    P_EMPLOYEE_NAME,
                    P_CONTACT_MOBILE,
                    P_TYPE_FLAG,
                    P_VALUE_FLAG,
                    P_REF_ORDER_NO,
                    P_REMARK,
                    P_CLIENT_ID,

                    RETURN_CODE,
                    RETURN_MESSAGE
                });

                command.ReturnCode = RETURN_CODE.Value.ToString() == "0" ? 200 : 400;
                command.ReturnMessage = Convert.ToString(RETURN_MESSAGE.Value);

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command.ReturnCode, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
                command.ReturnCode = 500;
                command.ReturnMessage = "Error InsertConsentLogNewRegisterCommandHandle: " + ex.StackTrace;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", ex.StackTrace, "");
            }
        }
    }
}
