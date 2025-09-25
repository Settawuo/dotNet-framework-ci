using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.ExWebServices.FbbCpGw;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw
{
    public class QueryLOVForWebHandler : IQueryHandler<QueryLOVForWebQuery, QueryLOVForWebModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<QueryLOVForWebData> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public QueryLOVForWebHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<QueryLOVForWebData> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public QueryLOVForWebModel Handle(QueryLOVForWebQuery query)
        {
            InterfaceLogCommand log = null;
            QueryLOVForWebModel result = new QueryLOVForWebModel();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "",
                    "QueryLOVForWeb", "QueryLOVForWebHandler", "", "FBB", "");

                //input
                var LovType = new OracleParameter();
                LovType.ParameterName = "LovType";
                LovType.OracleDbType = OracleDbType.Varchar2;
                LovType.Size = 2000;
                LovType.Direction = ParameterDirection.Input;
                LovType.Value = query.LovType.ToSafeString();

                var LovName = new OracleParameter();
                LovName.ParameterName = "LovName";
                LovName.OracleDbType = OracleDbType.Varchar2;
                LovName.Size = 2000;
                LovName.Direction = ParameterDirection.Input;
                LovName.Value = query.LovName.ToSafeString();

                //output
                var ReturnCode = new OracleParameter();
                ReturnCode.ParameterName = "ReturnCode";
                ReturnCode.OracleDbType = OracleDbType.Varchar2;
                ReturnCode.Size = 2000;
                ReturnCode.Direction = ParameterDirection.Output;

                var ReturnMessage = new OracleParameter();
                ReturnMessage.ParameterName = "ReturnMessage";
                ReturnMessage.OracleDbType = OracleDbType.Varchar2;
                ReturnMessage.Size = 2000;
                ReturnMessage.Direction = ParameterDirection.Output;

                var LIST_LOV_CUR = new OracleParameter();
                LIST_LOV_CUR.ParameterName = "LIST_LOV_CUR";
                LIST_LOV_CUR.OracleDbType = OracleDbType.RefCursor;
                LIST_LOV_CUR.Direction = ParameterDirection.Output;

                List<QueryLOVForWebData> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_QUERY_CONFIG_TIMESLOT.PROC_QUERY_LOV",

                            new
                            {
                                LovType,
                                LovName,

                                //out
                                ReturnCode,
                                ReturnMessage,
                                LIST_LOV_CUR

                            }).ToList();


                result.LIST_LOV_CUR = executeResult;
                result.ReturnCode = ReturnCode.Value != null ? ReturnCode.Value.ToSafeString() : "-1";
                result.ReturnMessage = ReturnMessage.Value != null ? ReturnMessage.Value.ToSafeString() : "";

                if (result.ReturnCode == "0")
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", "resultExecute is null", "");
                }


                return result;

            }
            catch (Exception ex)
            {
                result.ReturnCode = "-1";
                result.ReturnMessage = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Failed", ex.GetBaseException().ToString(), "");

                return result;
            }

        }

    }
}
