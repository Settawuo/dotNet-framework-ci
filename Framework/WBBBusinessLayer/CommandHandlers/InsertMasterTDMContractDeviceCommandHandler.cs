using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.CommandHandlers
{
    public class InsertMasterTDMContractDeviceCommandHandler : ICommandHandler<InsertMasterTDMContractDeviceCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public InsertMasterTDMContractDeviceCommandHandler(ILogger logger,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(InsertMasterTDMContractDeviceCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.Transaction_Id, "InsertMasterTDM", "InsertMasterTDMContractDeviceCommandHandler", null, "FBB", "");

                var P_RESULT_CODE_TDM = new OracleParameter();
                P_RESULT_CODE_TDM.ParameterName = "P_RESULT_CODE_TDM";
                P_RESULT_CODE_TDM.Size = 2000;
                P_RESULT_CODE_TDM.OracleDbType = OracleDbType.Varchar2;
                P_RESULT_CODE_TDM.Direction = ParameterDirection.Input;
                P_RESULT_CODE_TDM.Value = command.P_RESULT_CODE_TDM.ToSafeString();

                var P_CONTRACT_ID = new OracleParameter();
                P_CONTRACT_ID.ParameterName = "P_CONTRACT_ID";
                P_CONTRACT_ID.Size = 2000;
                P_CONTRACT_ID.OracleDbType = OracleDbType.Varchar2;
                P_CONTRACT_ID.Direction = ParameterDirection.Input;
                P_CONTRACT_ID.Value = command.P_CONTRACT_ID.ToSafeString();

                var P_CONTRACT_NAME = new OracleParameter();
                P_CONTRACT_NAME.ParameterName = "P_CONTRACT_NAME";
                P_CONTRACT_NAME.Size = 2000;
                P_CONTRACT_NAME.OracleDbType = OracleDbType.Varchar2;
                P_CONTRACT_NAME.Direction = ParameterDirection.Input;
                P_CONTRACT_NAME.Value = command.P_CONTRACT_NAME.ToSafeString();

                var P_CONTRACT_TYPE = new OracleParameter();
                P_CONTRACT_TYPE.ParameterName = "P_CONTRACT_TYPE";
                P_CONTRACT_TYPE.Size = 2000;
                P_CONTRACT_TYPE.OracleDbType = OracleDbType.Varchar2;
                P_CONTRACT_TYPE.Direction = ParameterDirection.Input;
                P_CONTRACT_TYPE.Value = command.P_CONTRACT_TYPE.ToSafeString();

                var P_CONTRACT_RULE_ID = new OracleParameter();
                P_CONTRACT_RULE_ID.ParameterName = "P_CONTRACT_RULE_ID";
                P_CONTRACT_RULE_ID.Size = 2000;
                P_CONTRACT_RULE_ID.OracleDbType = OracleDbType.Varchar2;
                P_CONTRACT_RULE_ID.Direction = ParameterDirection.Input;
                P_CONTRACT_RULE_ID.Value = command.P_CONTRACT_RULE_ID.ToSafeString();

                var P_PENALTY_TYPE = new OracleParameter();
                P_PENALTY_TYPE.ParameterName = "P_PENALTY_TYPE";
                P_PENALTY_TYPE.Size = 2000;
                P_PENALTY_TYPE.OracleDbType = OracleDbType.Varchar2;
                P_PENALTY_TYPE.Direction = ParameterDirection.Input;
                P_PENALTY_TYPE.Value = command.P_PENALTY_TYPE.ToSafeString();

                var P_PENALTY_ID = new OracleParameter();
                P_PENALTY_ID.ParameterName = "P_PENALTY_ID";
                P_PENALTY_ID.Size = 2000;
                P_PENALTY_ID.OracleDbType = OracleDbType.Varchar2;
                P_PENALTY_ID.Direction = ParameterDirection.Input;
                P_PENALTY_ID.Value = command.P_PENALTY_ID.ToSafeString();

                var P_LIMIT_CONTRACT = new OracleParameter();
                P_LIMIT_CONTRACT.ParameterName = "P_LIMIT_CONTRACT";
                P_LIMIT_CONTRACT.Size = 2000;
                P_LIMIT_CONTRACT.OracleDbType = OracleDbType.Varchar2;
                P_LIMIT_CONTRACT.Direction = ParameterDirection.Input;
                P_LIMIT_CONTRACT.Value = command.P_LIMIT_CONTRACT.ToSafeString();

                var P_DURATION = new OracleParameter();
                P_DURATION.ParameterName = "P_DURATION";
                P_DURATION.Size = 2000;
                P_DURATION.OracleDbType = OracleDbType.Varchar2;
                P_DURATION.Direction = ParameterDirection.Input;
                P_DURATION.Value = command.P_DURATION.ToSafeString();

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.Size = 2000;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var LIST_PARA_SAVE = new OracleParameter();
                LIST_PARA_SAVE.ParameterName = "list_para_save";
                LIST_PARA_SAVE.OracleDbType = OracleDbType.RefCursor;
                LIST_PARA_SAVE.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_DEVICECONTRACT_NEW.PROC_INSERT_MASTER_TDM",
                    new object[]
                    {
                        //in 
                        P_RESULT_CODE_TDM,
                        P_CONTRACT_ID,
                        P_CONTRACT_NAME,
                        P_CONTRACT_TYPE,
                        P_CONTRACT_RULE_ID,
                        P_PENALTY_TYPE,
                        P_PENALTY_ID,
                        P_LIMIT_CONTRACT,
                        P_DURATION,

                        /// Out
                        RETURN_CODE,
                        RETURN_MESSAGE,
                        LIST_PARA_SAVE

                    });
                command.RETURN_CODE = result[0] != null ? result[0].ToSafeString() : "-1";
                command.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "error";

                if (command.RETURN_CODE != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    command.LIST_PARA_SAVE = data1.DataTableToList<InsertMasterTDMModel>();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", command.RETURN_MESSAGE, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(),
                        "");
                }

                command.RETURN_CODE = "-1";
                command.RETURN_MESSAGE = "Error InsertMasterTDMContractDeviceCommand " + ex.Message;
            }
        }
    }

    public class InsertMasterTDMContractDeviceMeshCommandHandler : ICommandHandler<InsertMasterTDMContractDeviceMeshCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public InsertMasterTDMContractDeviceMeshCommandHandler(ILogger logger,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _intfLog = intfLog;
            _uow = uow;
        }

        public void Handle(InsertMasterTDMContractDeviceMeshCommand command)
        {
            InterfaceLogCommand log = null;
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.Transaction_Id, "InsertMasterTDMMesh", "InsertMasterTDMContractDeviceMeshCommandHandler", null, "FBB", "");

                var P_RESULT_CODE_TDM = new OracleParameter();
                P_RESULT_CODE_TDM.ParameterName = "P_RESULT_CODE_TDM";
                P_RESULT_CODE_TDM.Size = 2000;
                P_RESULT_CODE_TDM.OracleDbType = OracleDbType.Varchar2;
                P_RESULT_CODE_TDM.Direction = ParameterDirection.Input;
                P_RESULT_CODE_TDM.Value = command.P_RESULT_CODE_TDM.ToSafeString();

                var P_CONTRACT_ID = new OracleParameter();
                P_CONTRACT_ID.ParameterName = "P_CONTRACT_ID";
                P_CONTRACT_ID.Size = 2000;
                P_CONTRACT_ID.OracleDbType = OracleDbType.Varchar2;
                P_CONTRACT_ID.Direction = ParameterDirection.Input;
                P_CONTRACT_ID.Value = command.P_CONTRACT_ID.ToSafeString();

                var P_CONTRACT_NAME = new OracleParameter();
                P_CONTRACT_NAME.ParameterName = "P_CONTRACT_NAME";
                P_CONTRACT_NAME.Size = 2000;
                P_CONTRACT_NAME.OracleDbType = OracleDbType.Varchar2;
                P_CONTRACT_NAME.Direction = ParameterDirection.Input;
                P_CONTRACT_NAME.Value = command.P_CONTRACT_NAME.ToSafeString();

                var P_CONTRACT_TYPE = new OracleParameter();
                P_CONTRACT_TYPE.ParameterName = "P_CONTRACT_TYPE";
                P_CONTRACT_TYPE.Size = 2000;
                P_CONTRACT_TYPE.OracleDbType = OracleDbType.Varchar2;
                P_CONTRACT_TYPE.Direction = ParameterDirection.Input;
                P_CONTRACT_TYPE.Value = command.P_CONTRACT_TYPE.ToSafeString();

                var P_CONTRACT_RULE_ID = new OracleParameter();
                P_CONTRACT_RULE_ID.ParameterName = "P_CONTRACT_RULE_ID";
                P_CONTRACT_RULE_ID.Size = 2000;
                P_CONTRACT_RULE_ID.OracleDbType = OracleDbType.Varchar2;
                P_CONTRACT_RULE_ID.Direction = ParameterDirection.Input;
                P_CONTRACT_RULE_ID.Value = command.P_CONTRACT_RULE_ID.ToSafeString();

                var P_PENALTY_TYPE = new OracleParameter();
                P_PENALTY_TYPE.ParameterName = "P_PENALTY_TYPE";
                P_PENALTY_TYPE.Size = 2000;
                P_PENALTY_TYPE.OracleDbType = OracleDbType.Varchar2;
                P_PENALTY_TYPE.Direction = ParameterDirection.Input;
                P_PENALTY_TYPE.Value = command.P_PENALTY_TYPE.ToSafeString();

                var P_PENALTY_ID = new OracleParameter();
                P_PENALTY_ID.ParameterName = "P_PENALTY_ID";
                P_PENALTY_ID.Size = 2000;
                P_PENALTY_ID.OracleDbType = OracleDbType.Varchar2;
                P_PENALTY_ID.Direction = ParameterDirection.Input;
                P_PENALTY_ID.Value = command.P_PENALTY_ID.ToSafeString();

                var P_LIMIT_CONTRACT = new OracleParameter();
                P_LIMIT_CONTRACT.ParameterName = "P_LIMIT_CONTRACT";
                P_LIMIT_CONTRACT.Size = 2000;
                P_LIMIT_CONTRACT.OracleDbType = OracleDbType.Varchar2;
                P_LIMIT_CONTRACT.Direction = ParameterDirection.Input;
                P_LIMIT_CONTRACT.Value = command.P_LIMIT_CONTRACT.ToSafeString();

                var P_COUNT_FLAG = new OracleParameter();
                P_COUNT_FLAG.ParameterName = "P_COUNT_FLAG";
                P_COUNT_FLAG.Size = 2000;
                P_COUNT_FLAG.OracleDbType = OracleDbType.Varchar2;
                P_COUNT_FLAG.Direction = ParameterDirection.Input;
                P_COUNT_FLAG.Value = command.P_COUNT_FLAG.ToSafeString();

                var P_DURATION = new OracleParameter();
                P_DURATION.ParameterName = "P_DURATION";
                P_DURATION.Size = 2000;
                P_DURATION.OracleDbType = OracleDbType.Varchar2;
                P_DURATION.Direction = ParameterDirection.Input;
                P_DURATION.Value = command.P_DURATION.ToSafeString();

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.Size = 2000;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var LIST_DEVICE_CONTRACT = new OracleParameter();
                LIST_DEVICE_CONTRACT.ParameterName = "LIST_DEVICE_CONTRACT";
                LIST_DEVICE_CONTRACT.OracleDbType = OracleDbType.RefCursor;
                LIST_DEVICE_CONTRACT.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR041.PROC_INSERT_MASTER_TDM",
                    new object[]
                    {
                        //in 
                        P_RESULT_CODE_TDM,
                        P_CONTRACT_ID,
                        P_CONTRACT_NAME,
                        P_CONTRACT_TYPE,
                        P_CONTRACT_RULE_ID,
                        P_PENALTY_TYPE,
                        P_PENALTY_ID,
                        P_LIMIT_CONTRACT,
                        P_COUNT_FLAG,
                        P_DURATION,

                        /// Out
                        RETURN_CODE,
                        RETURN_MESSAGE,
                        LIST_DEVICE_CONTRACT

                    });
                command.RETURN_CODE = result[0] != null ? result[0].ToSafeString() : "-1";
                command.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "error";

                if (command.RETURN_CODE != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    command.LIST_DEVICE_CONTRACT = data1.DataTableToList<DeviceContractMeshData>();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, command, log, "Failed", command.RETURN_MESSAGE, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(),
                        "");
                }

                command.RETURN_CODE = "-1";
                command.RETURN_MESSAGE = "Error InsertMasterTDMContractDeviceCommand " + ex.Message;
            }
        }
    }
}
