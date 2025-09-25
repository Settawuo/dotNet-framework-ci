using AIRNETEntity.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    public class GetTableAirFbbHandler : IQueryHandler<GetTableAirFbbQuery, List<GetTableAirFbbModel>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;



        public GetTableAirFbbHandler(ILogger logger,
        IWBBUnitOfWork uow,
        IEntityRepository<FBB_INTERFACE_LOG> intfLog,
        IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public List<GetTableAirFbbModel> Handle(GetTableAirFbbQuery query)
        {
            List<GetTableAirFbbModel> executeResults = new List<GetTableAirFbbModel>();
            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.SFF_PROMOTION_CODE, "GetTableAirFbb", "GetTableAirFbbHandler", null, "FBB", "");
            try
            {

                var P_ACTION = new OracleParameter();
                P_ACTION.ParameterName = "P_ACTION";
                P_ACTION.Size = 2000;
                P_ACTION.OracleDbType = OracleDbType.Varchar2;
                P_ACTION.Direction = ParameterDirection.Input;
                P_ACTION.Value = query.ACTION;

                var P_SFF_PROMOTION_CODE = new OracleParameter();
                P_SFF_PROMOTION_CODE.ParameterName = "P_SFF_PROMOTION_CODE";
                P_SFF_PROMOTION_CODE.Size = 2000;
                P_SFF_PROMOTION_CODE.OracleDbType = OracleDbType.Varchar2;
                P_SFF_PROMOTION_CODE.Direction = ParameterDirection.Input;
                P_SFF_PROMOTION_CODE.Value = query.SFF_PROMOTION_CODE;

                var P_SERVICE_CODE = new OracleParameter();
                P_SERVICE_CODE.ParameterName = "P_SERVICE_CODE";
                P_SERVICE_CODE.Size = 2000;
                P_SERVICE_CODE.OracleDbType = OracleDbType.Varchar2;
                P_SERVICE_CODE.Direction = ParameterDirection.Input;
                P_SERVICE_CODE.Value = query.SERVICE_CODE;
                
                var P_PACKAGE_TYPE = new OracleParameter();
                P_PACKAGE_TYPE.ParameterName = "P_PACKAGE_TYPE";
                P_PACKAGE_TYPE.Size = 2000;
                P_PACKAGE_TYPE.OracleDbType = OracleDbType.Varchar2;
                P_PACKAGE_TYPE.Direction = ParameterDirection.Input;
                P_PACKAGE_TYPE.Value = query.PACKAGE_TYPE;

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var RETURN_EXTERNAL_DATA_CURROR = new OracleParameter();
                RETURN_EXTERNAL_DATA_CURROR.ParameterName = "RETURN_EXTERNAL_DATA_CURROR";
                RETURN_EXTERNAL_DATA_CURROR.OracleDbType = OracleDbType.RefCursor;
                RETURN_EXTERNAL_DATA_CURROR.Direction = ParameterDirection.Output;


                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_EXT_GET_MASTER_AIR_ADMIN.GET_EXTERNAL_MASTER",
                    new object[]
                    {
                         P_ACTION,
                         P_SFF_PROMOTION_CODE,
                         P_SERVICE_CODE,
                         P_PACKAGE_TYPE,

                         //return code
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         RETURN_EXTERNAL_DATA_CURROR
                    });

                string returnCode = result[0] != null ? result[1].ToSafeString() : "-1";
                string returnMessage = result[1] != null ? result[2].ToSafeString() : "error";
                if (result.Count > 2)
                {
                    var d_Return = (DataTable)result[2];
                    executeResults = d_Return.DataTableToList<GetTableAirFbbModel>();
                    if (executeResults == null)
                    {
                        return executeResults;
                    }
                    return executeResults;
                }
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Failed", ex.GetBaseException().ToString(), "");
            }
            return executeResults;
        }
    }
}
