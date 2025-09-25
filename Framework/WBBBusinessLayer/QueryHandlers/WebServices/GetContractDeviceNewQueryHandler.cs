using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetContractDeviceNewQueryHandler : IQueryHandler<GetContractDeviceNewQuery, GetContractDeviceNewModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetContractDeviceNewQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public GetContractDeviceNewModel Handle(GetContractDeviceNewQuery query)
        {
            InterfaceLogCommand log = null;
            GetContractDeviceNewModel executeResults = new GetContractDeviceNewModel();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "ContractDeviceNew", "GetContractDeviceNewQueryHandler", null, "FBB", "");

                var P_PROMOTION_CODE_MAIN = new OracleParameter();
                P_PROMOTION_CODE_MAIN.ParameterName = "P_PROMOTION_CODE_MAIN";
                P_PROMOTION_CODE_MAIN.Size = 2000;
                P_PROMOTION_CODE_MAIN.OracleDbType = OracleDbType.Varchar2;
                P_PROMOTION_CODE_MAIN.Direction = ParameterDirection.Input;
                P_PROMOTION_CODE_MAIN.Value = query.P_PROMOTION_CODE_MAIN.ToSafeString();

                var P_MESH_FLAG = new OracleParameter();
                P_MESH_FLAG.ParameterName = "P_MESH_FLAG";
                P_MESH_FLAG.Size = 2000;
                P_MESH_FLAG.OracleDbType = OracleDbType.Varchar2;
                P_MESH_FLAG.Direction = ParameterDirection.Input;
                P_MESH_FLAG.Value = query.P_MESH_FLAG.ToSafeString();

                var P_DURATION = new OracleParameter();
                P_DURATION.ParameterName = "P_DURATION";
                P_DURATION.Size = 2000;
                P_DURATION.OracleDbType = OracleDbType.Varchar2;
                P_DURATION.Direction = ParameterDirection.Input;
                P_DURATION.Value = query.P_DURATION.ToSafeString();

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.Size = 10;
                RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var LIST_MASTER = new OracleParameter();
                LIST_MASTER.ParameterName = "list_master";
                LIST_MASTER.OracleDbType = OracleDbType.RefCursor;
                LIST_MASTER.Direction = ParameterDirection.Output;

                _logger.Info("Start PKG_FBB_CONTRACTDEVICE_NEW.PROC_COMPARE_MASTER_MAPPING");

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_DEVICECONTRACT_NEW.PROC_COMPARE_MASTER_MAPPING",
                    new object[]
                    {
                         P_PROMOTION_CODE_MAIN,
                         P_MESH_FLAG,
                         P_DURATION,

                         //return code
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         LIST_MASTER
                    });

                executeResults.RETURN_CODE = result[0] != null ? result[0].ToSafeString() : "-1";
                executeResults.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "error";

                if (executeResults.RETURN_CODE != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    executeResults.LIST_MASTER = data1.DataTableToList<ContractDeviceNewModel>();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Failed", executeResults.RETURN_MESSAGE, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBB_CONTRACTDEVICE_NEW.PROC_COMPARE_MASTER_MAPPING handles : " + ex.Message);

                executeResults.RETURN_CODE = "-1";
                executeResults.RETURN_MESSAGE = "Error";

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults.RETURN_MESSAGE, log, "Failed", executeResults.RETURN_MESSAGE, "");

                return null;
            }
            return executeResults;
        }
    }
}
