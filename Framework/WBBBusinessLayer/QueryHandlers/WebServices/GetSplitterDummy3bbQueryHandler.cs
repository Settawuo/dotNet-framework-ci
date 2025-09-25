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
    public class GetSplitterDummy3bbQueryHandler : IQueryHandler<GetSplitterDummy3bbQuery, GetSplitterDummy3bbQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetSplitterDummy3bbQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public GetSplitterDummy3bbQueryModel Handle(GetSplitterDummy3bbQuery query)
        {
            InterfaceLogCommand log = null;
            GetSplitterDummy3bbQueryModel executeResults = new GetSplitterDummy3bbQueryModel();
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "GetSplitterDummy3bbQuery", "GetSplitterDummy3bbQueryHandler", null, "FBB", "");

                var P_ADDRESS_ID = new OracleParameter();
                P_ADDRESS_ID.ParameterName = "P_ADDRESS_ID";
                P_ADDRESS_ID.Size = 2000;
                P_ADDRESS_ID.OracleDbType = OracleDbType.Varchar2;
                P_ADDRESS_ID.Direction = ParameterDirection.Input;
                P_ADDRESS_ID.Value = query.p_address_id.ToSafeString();

                var P_RETURN_CODE = new OracleParameter();
                P_RETURN_CODE.ParameterName = "P_RETURN_CODE";
                P_RETURN_CODE.Size = 10;
                P_RETURN_CODE.OracleDbType = OracleDbType.Varchar2;
                P_RETURN_CODE.Direction = ParameterDirection.Output;

                var P_RETURN_MESSAGE = new OracleParameter();
                P_RETURN_MESSAGE.ParameterName = "P_RETURN_MESSAGE";
                P_RETURN_MESSAGE.Size = 2000;
                P_RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                P_RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var RETURN_SPLITTER = new OracleParameter();
                RETURN_SPLITTER.ParameterName = "RETURN_SPLITTER";
                RETURN_SPLITTER.OracleDbType = OracleDbType.RefCursor;
                RETURN_SPLITTER.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_INTERFACE_LOG_3BB.PROC_QUERY_SPLITTER",
                    new object[]
                    {
                         P_ADDRESS_ID,

                         //return code
                         P_RETURN_CODE,
                         P_RETURN_MESSAGE,
                         RETURN_SPLITTER
                    });

                executeResults.ReturnCode = result[0] != null ? result[0].ToSafeString() : "-1";
                executeResults.ReturnMsg = result[1] != null ? result[1].ToSafeString() : "error";

                if (executeResults.ReturnCode != "-1")
                {
                    DataTable data1 = (DataTable)result[2];
                    executeResults.Data = data1.DataTableToList<GetSplitterDummy3bbQueryModelData>();
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Failed", executeResults.ReturnMsg, "");
                }

            }
            catch (Exception ex)
            {
                executeResults.ReturnMsg = "Error";

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex.Message, log, "Failed", executeResults.ReturnMsg, "");

                return null;
            }
            return executeResults;
        }
    }
}
