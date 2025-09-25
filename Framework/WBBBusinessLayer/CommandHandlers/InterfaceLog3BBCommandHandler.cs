using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class InterfaceLog3BBCommandHandler : ICommandHandler<InterfaceLog3BBCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_3BB> _interfaceLog;
        private readonly IEntityRepository<string> _objService;

        public InterfaceLog3BBCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG_3BB> interfaceLog,
            IEntityRepository<string> objService)
        {
            _logger = logger;
            _uow = uow;
            _interfaceLog = interfaceLog;
            _objService = objService;
        }

        public void Handle(InterfaceLog3BBCommand command)
        {
            var log = InterfaceLog3BBHelper.Log(_interfaceLog, command);
            //_uow.Persist();
            command.OutInterfaceLogId = log.INTERFACE_ID;
        }
    }

    public static class InterfaceLog3BBHelper
    {
        public static FBB_INTERFACE_LOG_3BB Log(IEntityRepository<FBB_INTERFACE_LOG_3BB> interfaceLog,
            InterfaceLog3BBCommand command)
        {
            try
            {
                if (command.ActionType == ActionType.Insert)
                {
                    #region DirectInsert
                    //var data = new FBB_INTERFACE_LOG_3BB();
                    //data.IN_TRANSACTION_ID = command.IN_TRANSACTION_ID;
                    //data.METHOD_NAME = command.METHOD_NAME;
                    //data.SERVICE_NAME = command.SERVICE_NAME;
                    //data.IN_ID_CARD_NO = command.IN_ID_CARD_NO;
                    //data.IN_XML_PARAM = command.IN_XML_PARAM;
                    //data.OUT_RESULT = command.OUT_RESULT;
                    //data.OUT_ERROR_RESULT = command.OUT_ERROR_RESULT;
                    //data.OUT_XML_PARAM = command.OUT_XML_PARAM;
                    //data.REQUEST_STATUS = "PENDING";
                    //data.INTERFACE_NODE = command.INTERFACE_NODE;
                    //data.CREATED_BY = command.CREATED_BY;
                    //data.CREATED_DATE = DateTime.Now;

                    //interfaceLog.Create(data);
                    //return data;
                    #endregion

                    #region UsePackageInsert

                    var data = new FBB_INTERFACE_LOG_3BB();
                    data.IN_TRANSACTION_ID = command.IN_TRANSACTION_ID;
                    data.METHOD_NAME = command.METHOD_NAME;
                    data.SERVICE_NAME = command.SERVICE_NAME;
                    data.IN_ID_CARD_NO = command.IN_ID_CARD_NO;
                    data.IN_XML_PARAM = command.IN_XML_PARAM;
                    data.REQUEST_STATUS = "PENDING";
                    data.INTERFACE_NODE = command.INTERFACE_NODE;

                    var outp = new List<object>();
                    var paramOut = outp.ToArray();

                    var bufferData = new OracleParameter();
                    bufferData.OracleDbType = OracleDbType.Clob;
                    bufferData.Value = command.IN_XML_PARAM;
                    bufferData.Direction = ParameterDirection.Input;

                    var ret_code = new OracleParameter();
                    ret_code.OracleDbType = OracleDbType.Varchar2;
                    ret_code.Size = 2000;
                    ret_code.Direction = ParameterDirection.Output;

                    var ret_message = new OracleParameter();
                    ret_message.OracleDbType = OracleDbType.Varchar2;
                    ret_message.Size = 2000;
                    ret_message.Direction = ParameterDirection.Output;

                    var interface_id = new OracleParameter();
                    interface_id.OracleDbType = OracleDbType.Varchar2;
                    interface_id.Size = 2000;
                    interface_id.Direction = ParameterDirection.Output;

                    // Update 17.2
                    string in_transection_id = command.IN_TRANSACTION_ID;
                    if (command.IN_TRANSACTION_ID != null)
                    {
                        if ((command.IN_TRANSACTION_ID.Contains('.') == true || command.IN_TRANSACTION_ID.Contains(':') == true) && command.IN_TRANSACTION_ID.Contains('|') == false)
                        {
                            in_transection_id = command.IN_TRANSACTION_ID.Substring(0, 10);
                        }
                        if (command.IN_TRANSACTION_ID.Contains('|') == true)
                        {
                            in_transection_id = command.IN_TRANSACTION_ID.Replace("|", "");
                        }
                    }

                    var executeResult = interfaceLog.ExecuteStoredProc("WBB.PKG_INTERFACE_LOG_3BB.PROC_INSERT_INTERFACE_LOG_3BB",
                    out paramOut,
                    new
                    {
                        p_in_transaction_id = in_transection_id,
                        p_method_name = command.METHOD_NAME,
                        p_service_name = command.SERVICE_NAME,
                        p_in_id_card_no = command.IN_ID_CARD_NO,
                        p_in_xml_param = bufferData,
                        p_request_status = "PENDING",
                        p_interface_node = command.INTERFACE_NODE,

                        //// return 
                        p_return_code = ret_code,
                        p_return_message = ret_message,
                        p_interface_id = interface_id
                    });
                    command.INTERFACE_NODE = interface_id.Value.ToSafeString();
                    data.INTERFACE_ID = long.Parse(command.INTERFACE_NODE);
                    return data;
                    #endregion
                }
                else if (command.ActionType == ActionType.Update)
                {
                    #region DirectUpdate
                    //var log = (from t in interfaceLog.Get()
                    //           where t.INTERFACE_ID == command.OutInterfaceLogId
                    //           select t).FirstOrDefault();

                    //if (null == log)
                    //    throw new Exception("InterfaceLog3BBCommand Error : Transaction ID Not Found " + command.IN_TRANSACTION_ID);

                    //log.REQUEST_STATUS = command.REQUEST_STATUS;
                    //log.OUT_RESULT = command.OUT_RESULT;
                    //log.OUT_ERROR_RESULT = command.OUT_ERROR_RESULT;
                    //log.OUT_XML_PARAM = command.OUT_XML_PARAM;

                    //log.UPDATED_BY = "FBB";
                    //log.UPDATED_DATE = DateTime.Now;

                    //interfaceLog.Update(log, interfaceLog.GetByKey(command.OutInterfaceLogId));
                    //return log;
                    #endregion

                    #region UsePackageUpdate
                    var log = new FBB_INTERFACE_LOG_3BB();

                    log.REQUEST_STATUS = command.REQUEST_STATUS;
                    log.OUT_RESULT = command.OUT_RESULT;
                    log.OUT_ERROR_RESULT = command.OUT_ERROR_RESULT;
                    log.OUT_XML_PARAM = command.OUT_XML_PARAM;
                    log.UPDATED_BY = command.UPDATED_BY;

                    var outp = new List<object>();
                    var paramOut = outp.ToArray();

                    var bufferData = new OracleParameter();
                    bufferData.OracleDbType = OracleDbType.Clob;
                    bufferData.Value = command.OUT_XML_PARAM;
                    bufferData.Direction = ParameterDirection.Input;

                    var ret_code = new OracleParameter();
                    ret_code.OracleDbType = OracleDbType.Varchar2;
                    ret_code.Size = 2000;
                    ret_code.Direction = ParameterDirection.Output;

                    var ret_message = new OracleParameter();
                    ret_message.OracleDbType = OracleDbType.Varchar2;
                    ret_message.Size = 2000;
                    ret_message.Direction = ParameterDirection.Output;

                    // Update 17.2
                    string in_transection_id = command.IN_TRANSACTION_ID;
                    string updated_by = command.UPDATED_BY;
                    if ((command.IN_TRANSACTION_ID.Contains('.') == true || command.IN_TRANSACTION_ID.Contains(':') == true) && command.IN_TRANSACTION_ID.Contains('|') == false)
                    {
                        if (command.IN_TRANSACTION_ID.Length >= 10)
                        {
                            in_transection_id = command.IN_TRANSACTION_ID.Substring(0, 10);
                            updated_by = command.IN_TRANSACTION_ID.Substring(10);
                        }
                    }
                    if (command.IN_TRANSACTION_ID.Contains('|') == true)
                    {
                        in_transection_id = command.IN_TRANSACTION_ID.Replace("|", "");
                    }

                    var executeResult = interfaceLog.ExecuteStoredProc("WBB.PKG_INTERFACE_LOG_3BB.PROC_UPDATE_INTERFACE_LOG_3BB",
                    out paramOut,
                    new
                    {
                        p_end_interface_id = command.OutInterfaceLogId.ToSafeString(),
                        p_request_status = command.REQUEST_STATUS,
                        p_out_result = command.OUT_RESULT,
                        p_out_error_result = command.OUT_ERROR_RESULT,
                        p_out_xml_param = bufferData,
                        p_updated_by = updated_by,

                        //// return 
                        p_return_code = ret_code,
                        p_return_message = ret_message,
                    });
                    return log;
                    #endregion
                }
                else
                {
                    throw new Exception("InterfaceLog3BBCommand Error");
                }
            }
            catch (Exception ex)
            {
                return new FBB_INTERFACE_LOG_3BB();
            }
        }
    }
}
