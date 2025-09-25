using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetMeshCompareDeviceOrderQueryHandler : IQueryHandler<GetMeshCompareDeviceOrderQuery, MeshCompareDeviceOrderModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetMeshCompareDeviceOrderQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public MeshCompareDeviceOrderModel Handle(GetMeshCompareDeviceOrderQuery query)
        {
            var log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.FibrenetID, "GetMeshCompareDeviceOrder", "GetMeshCompareDeviceOrderQueryHandler", null, "FBB", "");

            MeshCompareDeviceOrderModel executeResults = new MeshCompareDeviceOrderModel();
            try
            {

                var P_FIBRENET_ID = new OracleParameter();
                P_FIBRENET_ID.ParameterName = "p_fibrenet_id";
                P_FIBRENET_ID.Size = 2000;
                P_FIBRENET_ID.OracleDbType = OracleDbType.Varchar2;
                P_FIBRENET_ID.Direction = ParameterDirection.Input;
                P_FIBRENET_ID.Value = query.FibrenetID;

                var P_CHANNEL_NAME = new OracleParameter();
                P_CHANNEL_NAME.ParameterName = "p_channel_name";
                P_CHANNEL_NAME.Size = 2000;
                P_CHANNEL_NAME.OracleDbType = OracleDbType.Varchar2;
                P_CHANNEL_NAME.Direction = ParameterDirection.Input;
                P_CHANNEL_NAME.Value = query.Channel;

                var P_MESH_BRAND_NAME = new OracleParameter();
                P_MESH_BRAND_NAME.ParameterName = "p_mesh_brand_name";
                P_MESH_BRAND_NAME.Size = 2000;
                P_MESH_BRAND_NAME.OracleDbType = OracleDbType.Varchar2;
                P_MESH_BRAND_NAME.Direction = ParameterDirection.Input;
                P_MESH_BRAND_NAME.Value = query.MeshBrandName;

                var P_BUY_MESH = new OracleParameter();
                P_BUY_MESH.ParameterName = "p_buy_mesh";
                P_BUY_MESH.Size = 2000;
                P_BUY_MESH.OracleDbType = OracleDbType.Varchar2;
                P_BUY_MESH.Direction = ParameterDirection.Input;
                P_BUY_MESH.Value = query.BuyMesh;

                var P_PENALTY_INSTALL = new OracleParameter();
                P_PENALTY_INSTALL.ParameterName = "p_penalty_install";
                P_PENALTY_INSTALL.Size = 2000;
                P_PENALTY_INSTALL.OracleDbType = OracleDbType.Varchar2;
                P_PENALTY_INSTALL.Direction = ParameterDirection.Input;
                P_PENALTY_INSTALL.Value = query.PenaltyInstall;

                var P_CONTRACT_ID = new OracleParameter();
                P_CONTRACT_ID.ParameterName = "p_contract_id";
                P_CONTRACT_ID.Size = 2000;
                P_CONTRACT_ID.OracleDbType = OracleDbType.Varchar2;
                P_CONTRACT_ID.Direction = ParameterDirection.Input;
                P_CONTRACT_ID.Value = query.ContractID;

                var P_CONTRACT_NAME = new OracleParameter();
                P_CONTRACT_NAME.ParameterName = "p_contract_name";
                P_CONTRACT_NAME.Size = 2000;
                P_CONTRACT_NAME.OracleDbType = OracleDbType.Varchar2;
                P_CONTRACT_NAME.Direction = ParameterDirection.Input;
                P_CONTRACT_NAME.Value = query.ContractName;

                var P_DURATION = new OracleParameter();
                P_DURATION.ParameterName = "p_duration";
                P_DURATION.Size = 2000;
                P_DURATION.OracleDbType = OracleDbType.Varchar2;
                P_DURATION.Direction = ParameterDirection.Input;
                P_DURATION.Value = query.Duration;

                var P_CONTRACT_RULE_ID = new OracleParameter();
                P_CONTRACT_RULE_ID.ParameterName = "p_contract_rule_id";
                P_CONTRACT_RULE_ID.Size = 2000;
                P_CONTRACT_RULE_ID.OracleDbType = OracleDbType.Varchar2;
                P_CONTRACT_RULE_ID.Direction = ParameterDirection.Input;
                P_CONTRACT_RULE_ID.Value = query.ContractRuleId;

                var P_PENALTY_TYPE = new OracleParameter();
                P_PENALTY_TYPE.ParameterName = "p_penalty_type";
                P_PENALTY_TYPE.Size = 2000;
                P_PENALTY_TYPE.OracleDbType = OracleDbType.Varchar2;
                P_PENALTY_TYPE.Direction = ParameterDirection.Input;
                P_PENALTY_TYPE.Value = query.PenaltyType;

                var P_PENALTY_ID = new OracleParameter();
                P_PENALTY_ID.ParameterName = "p_penalty_id";
                P_PENALTY_ID.Size = 2000;
                P_PENALTY_ID.OracleDbType = OracleDbType.Varchar2;
                P_PENALTY_ID.Direction = ParameterDirection.Input;
                P_PENALTY_ID.Value = query.PenaltyId;

                var P_COUNT_FLAG = new OracleParameter();
                P_COUNT_FLAG.ParameterName = "p_count_flag";
                P_COUNT_FLAG.Size = 2000;
                P_COUNT_FLAG.OracleDbType = OracleDbType.Varchar2;
                P_COUNT_FLAG.Direction = ParameterDirection.Input;
                P_COUNT_FLAG.Value = query.CountFlag;

                var RETURN_FLAG_TDM = new OracleParameter();
                RETURN_FLAG_TDM.ParameterName = "return_flag_tdm";
                RETURN_FLAG_TDM.Size = 2000;
                RETURN_FLAG_TDM.OracleDbType = OracleDbType.Varchar2;
                RETURN_FLAG_TDM.Direction = ParameterDirection.Output;

                var RETURN_CONTRACT_ID = new OracleParameter();
                RETURN_CONTRACT_ID.ParameterName = "return_contract_id";
                RETURN_CONTRACT_ID.Size = 2000;
                RETURN_CONTRACT_ID.OracleDbType = OracleDbType.Varchar2;
                RETURN_CONTRACT_ID.Direction = ParameterDirection.Output;

                var RETURN_CONTRACT_NAME = new OracleParameter();
                RETURN_CONTRACT_NAME.ParameterName = "return_contract_name";
                RETURN_CONTRACT_NAME.Size = 2000;
                RETURN_CONTRACT_NAME.OracleDbType = OracleDbType.Varchar2;
                RETURN_CONTRACT_NAME.Direction = ParameterDirection.Output;

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "return_code";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "return_message";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var RETURN_CALL = new OracleParameter();
                RETURN_CALL.ParameterName = "return_paremeter";
                RETURN_CALL.OracleDbType = OracleDbType.RefCursor;
                RETURN_CALL.Direction = ParameterDirection.Output;

                _logger.Info("Start PKG_FBBOR041.COMPARE_DEVICE_ORDER");

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR041.COMPARE_DEVICE_ORDER",
                    new object[]
                    {
                         P_FIBRENET_ID,
                         P_CHANNEL_NAME,
                         P_MESH_BRAND_NAME,
                         P_BUY_MESH,
                         P_PENALTY_INSTALL,
                         P_CONTRACT_ID,
                         P_CONTRACT_NAME,
                         P_DURATION,
                         P_CONTRACT_RULE_ID,
                         P_PENALTY_TYPE,
                         P_PENALTY_ID,
                         P_COUNT_FLAG,               
                         //return code
                         RETURN_FLAG_TDM,
                         RETURN_CONTRACT_ID,
                         RETURN_CONTRACT_NAME,
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         RETURN_CALL
                    });
                executeResults.RETURN_FLAG_TDM = result[0] != null ? result[0].ToSafeString() : "";
                executeResults.RETURN_CONTRACT_ID = result[1] != null ? result[1].ToSafeString() : "";
                executeResults.RETURN_CONTRACT_NAME = result[2] != null ? result[2].ToSafeString() : "";
                executeResults.RETURN_CODE = result[3] != null ? result[3].ToSafeString() : "-1";
                executeResults.RETURN_MESSAGE = result[4] != null ? result[4].ToSafeString() : "error";
                if (executeResults.RETURN_CODE != "-1")
                {
                    DataTable data1 = (DataTable)result[5];
                    executeResults.RES_COMPLETE_CUR = data1.DataTableToList<CompareDeviceOrder>();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults.RETURN_MESSAGE, log, "Failed", executeResults.RETURN_MESSAGE, "");
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBBOR041.COMPARE_DEVICE_ORDER handles : " + ex.Message);

                executeResults.RETURN_CODE = "-1";
                executeResults.RETURN_MESSAGE = "Error";

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults.RETURN_MESSAGE, log, "Failed", executeResults.RETURN_MESSAGE, "");

                return null;
            }
            return executeResults;
        }
    }
}
