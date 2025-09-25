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
    public class GetDetailCalliMTopupReplaceQueryHandler : IQueryHandler<GetDetailCalliMTopupReplaceQuery, DetailCalliMTopupReplaceModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;

        public GetDetailCalliMTopupReplaceQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
        }

        public DetailCalliMTopupReplaceModel Handle(GetDetailCalliMTopupReplaceQuery query)
        {
            InterfaceLogCommand log = null;
            DetailCalliMTopupReplaceModel executeResults = new DetailCalliMTopupReplaceModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "DetailCalliMTopupReplace", "GetDetailCalliMTopupReplaceQueryHandler", "", "FBB|" + query.FullUrl, "WEB");

                var P_FIBRENET_ID = new OracleParameter();
                P_FIBRENET_ID.ParameterName = "P_FIBRENET_ID";
                P_FIBRENET_ID.OracleDbType = OracleDbType.Varchar2;
                P_FIBRENET_ID.Direction = ParameterDirection.Input;
                P_FIBRENET_ID.Value = query.P_FIBRENET_ID.ToSafeString();

                var P_CONTRACT_NO = new OracleParameter();
                P_CONTRACT_NO.ParameterName = "P_CONTRACT_NO";
                P_CONTRACT_NO.OracleDbType = OracleDbType.Varchar2;
                P_CONTRACT_NO.Direction = ParameterDirection.Input;
                P_CONTRACT_NO.Value = query.P_CONTRACT_NO.ToSafeString();

                var P_CUSTOMER_NAME = new OracleParameter();
                P_CUSTOMER_NAME.ParameterName = "P_CUSTOMER_NAME";
                P_CUSTOMER_NAME.OracleDbType = OracleDbType.Varchar2;
                P_CUSTOMER_NAME.Direction = ParameterDirection.Input;
                P_CUSTOMER_NAME.Value = query.P_CUSTOMER_NAME.ToSafeString();

                var P_SERIAL_NO = new OracleParameter();
                P_SERIAL_NO.ParameterName = "P_SERIAL_NO";
                P_SERIAL_NO.OracleDbType = OracleDbType.Varchar2;
                P_SERIAL_NO.Direction = ParameterDirection.Input;
                P_SERIAL_NO.Value = query.P_SERIAL_NO.ToSafeString();

                var P_RESERVED_ID = new OracleParameter();
                P_RESERVED_ID.ParameterName = "P_RESERVED_ID";
                P_RESERVED_ID.OracleDbType = OracleDbType.Varchar2;
                P_RESERVED_ID.Direction = ParameterDirection.Input;
                P_RESERVED_ID.Value = query.P_RESERVED_ID.ToSafeString();

                var P_TIME_SLOT = new OracleParameter();
                P_TIME_SLOT.ParameterName = "P_TIME_SLOT";
                P_TIME_SLOT.OracleDbType = OracleDbType.Varchar2;
                P_TIME_SLOT.Direction = ParameterDirection.Input;
                P_TIME_SLOT.Value = query.P_TIME_SLOT.ToSafeString();

                var P_DATE_TIME_SLOT = new OracleParameter();
                P_DATE_TIME_SLOT.ParameterName = "P_DATE_TIME_SLOT";
                P_DATE_TIME_SLOT.OracleDbType = OracleDbType.Varchar2;
                P_DATE_TIME_SLOT.Direction = ParameterDirection.Input;
                P_DATE_TIME_SLOT.Value = query.P_DATE_TIME_SLOT.ToSafeString();

                var P_ACCESS_MODE = new OracleParameter();
                P_ACCESS_MODE.ParameterName = "P_ACCESS_MODE";
                P_ACCESS_MODE.OracleDbType = OracleDbType.Varchar2;
                P_ACCESS_MODE.Direction = ParameterDirection.Input;
                P_ACCESS_MODE.Value = query.P_ACCESS_MODE.ToSafeString();

                var P_ADDRESS_ID = new OracleParameter();
                P_ADDRESS_ID.ParameterName = "P_ADDRESS_ID";
                P_ADDRESS_ID.OracleDbType = OracleDbType.Varchar2;
                P_ADDRESS_ID.Direction = ParameterDirection.Input;
                P_ADDRESS_ID.Value = query.P_ADDRESS_ID.ToSafeString();

                var P_COUNT_PB = new OracleParameter();
                P_COUNT_PB.ParameterName = "P_COUNT_PB";
                P_COUNT_PB.OracleDbType = OracleDbType.Varchar2;
                P_COUNT_PB.Direction = ParameterDirection.Input;
                P_COUNT_PB.Value = query.P_COUNT_PB.ToSafeString();

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

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR050.DETAIL_CALL_IM",
                     new object[]
                     {
                         P_FIBRENET_ID,
                         P_CONTRACT_NO,
                         P_CUSTOMER_NAME,
                         P_SERIAL_NO,
                         P_RESERVED_ID,
                         P_TIME_SLOT,
                         P_DATE_TIME_SLOT,
                         P_ACCESS_MODE,
                         P_ADDRESS_ID,
                         P_COUNT_PB,  

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
                        executeResults.RETURN_PRICE_CURROR = data1.DataTableToList<DetailCalliMTopupReplace>();
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
                _logger.Info("GetDetailCalliMTopupReplaceQueryHandler : Error.");
                _logger.Info(ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ex.StackTrace, "");
                throw;
            }

            return executeResults;
        }
    }
}
