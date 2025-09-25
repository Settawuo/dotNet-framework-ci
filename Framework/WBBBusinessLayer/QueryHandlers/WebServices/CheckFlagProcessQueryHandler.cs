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
    public class CheckFlagProcessQueryHandler : IQueryHandler<CheckFlagProcessQuery, CheckFlagProcessModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<CheckFlagProcessData> _checkFlagProcessData;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;

        public CheckFlagProcessQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<CheckFlagProcessData> checkFlagProcessData)
        {
            _logger = logger;
            _checkFlagProcessData = checkFlagProcessData;
            _intfLog = intfLog;
            _uow = uow;
        }

        public CheckFlagProcessModel Handle(CheckFlagProcessQuery query)
        {
            InterfaceLogCommand log = null;
            var RETURN_CODE = new OracleParameter();
            RETURN_CODE.ParameterName = "RETURN_CODE";
            RETURN_CODE.OracleDbType = OracleDbType.Decimal;
            RETURN_CODE.Direction = ParameterDirection.Output;

            var RETURN_MESSAGE = new OracleParameter();
            RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
            RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
            RETURN_MESSAGE.Size = 2000;
            RETURN_MESSAGE.Direction = ParameterDirection.Output;

            var RETURN_FLAG_KYC = new OracleParameter();
            RETURN_FLAG_KYC.ParameterName = "RETURN_FLAG_KYC";
            RETURN_FLAG_KYC.OracleDbType = OracleDbType.RefCursor;
            RETURN_FLAG_KYC.Direction = ParameterDirection.Output;

            CheckFlagProcessModel ResultData = new CheckFlagProcessModel();
            List<CheckFlagProcessData> executeResult = new List<CheckFlagProcessData>();

            try
            {

                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "CheckFlagProcess", "CheckFlagProcessQueryHandler", query.TRANSACTION_ID, "FBB|" + query.FULL_URL, "WEB");

                // 
                executeResult = _checkFlagProcessData.ExecuteReadStoredProc("WBB.PKG_FBBOR022.PROC_CHECK_FLAG",
                   new
                   {
                       P_TYPE = query.P_TYPE,
                       P_SUB_TYPE = query.P_SUB_TYPE,
                       P_MOBILE_TYPE = query.P_MOBILE_TYPE,
                       P_SERVICE_YEAR_BY_DAY = query.P_SERVICE_YEAR_BY_DAY,

                       // out
                       RETURN_CODE = RETURN_CODE,
                       RETURN_MESSAGE = RETURN_MESSAGE,
                       RETURN_FLAG_KYC = RETURN_FLAG_KYC
                   }).ToList();

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResult, log, "Success", "", "");

                ResultData.RETURN_CODE = RETURN_CODE.Value.ToSafeString();
                ResultData.RETURN_MESSAGE = RETURN_MESSAGE.Value.ToSafeString();
                if (executeResult.Count > 0)
                {
                    ResultData.checkFlagProcessData = executeResult.FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }
            }

            return ResultData;
        }

    }
}
