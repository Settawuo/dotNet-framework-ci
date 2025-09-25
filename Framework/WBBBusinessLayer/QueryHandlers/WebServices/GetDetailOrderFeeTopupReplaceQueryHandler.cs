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
    public class GetDetailOrderFeeTopupReplaceQueryHandler : IQueryHandler<GetDetailOrderFeeTopupReplaceQuery, DetailOrderFeeTopupReplaceModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetDetailOrderFeeTopupReplaceQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public DetailOrderFeeTopupReplaceModel Handle(GetDetailOrderFeeTopupReplaceQuery query)
        {
            InterfaceLogCommand log = null;
            DetailOrderFeeTopupReplaceModel executeResults = new DetailOrderFeeTopupReplaceModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "DetailOrderFeeTopupReplace", "GetDetailOrderFeeTopupReplaceQuery", "", "FBB|" + query.FullUrl, "WEB");

                var P_FLAG_LANG = new OracleParameter();
                P_FLAG_LANG.ParameterName = "P_FLAG_LANG";
                P_FLAG_LANG.OracleDbType = OracleDbType.Varchar2;
                P_FLAG_LANG.Direction = ParameterDirection.Input;
                P_FLAG_LANG.Value = query.P_FLAG_LANG;

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var RETURN_PRICE_CURROR = new OracleParameter();
                RETURN_PRICE_CURROR.ParameterName = "RETURN_PRICE_CURROR";
                RETURN_PRICE_CURROR.OracleDbType = OracleDbType.RefCursor;
                RETURN_PRICE_CURROR.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR050.DETAIL_ORDER_FEE",
                     new object[]
                     {
                         P_FLAG_LANG,

                         //return code
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         RETURN_PRICE_CURROR
                     });

                if (result != null)
                {
                    executeResults.RETURN_CODE = result[0] != null ? Convert.ToInt16(result[0].ToSafeString()) : -1;
                    executeResults.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "Error";

                    if (executeResults.RETURN_CODE != -1)
                    {
                        DataTable data1 = (DataTable)result[2];
                        executeResults.RETURN_PRICE_CURROR = data1.DataTableToList<PriceATV>();
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Success", "", "");
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Failed", executeResults.RETURN_MESSAGE, "");
                    }
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Failed", "Error", "");
                }
            }
            catch (Exception ex)
            {
                _logger.Info("GetDetailOrderFeeTopupReplaceQueryHandler : Error.");
                _logger.Info(ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ex.StackTrace, "");
                throw;
            }

            return executeResults;
        }

    }
}
