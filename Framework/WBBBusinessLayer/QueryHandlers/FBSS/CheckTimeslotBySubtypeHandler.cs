using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBSS;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.FBSSModels;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBSS
{
    public class CheckTimeslotBySubtypeHandler : IQueryHandler<CheckTimeslotBySubtypeDataQuery, CheckTimeslotBySubtypeModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ReturnConfigTimeslotModel> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public CheckTimeslotBySubtypeHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<ReturnConfigTimeslotModel> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public CheckTimeslotBySubtypeModel Handle(CheckTimeslotBySubtypeDataQuery query)
        {
            InterfaceLogCommand log = null;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionID, "CheckTimeslotBySubtype", "CheckTimeslotBySubtypeHandler", null, "FBB", "");

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

            var ReturnConfigTimeslot = new OracleParameter();
            ReturnConfigTimeslot.OracleDbType = OracleDbType.RefCursor;
            ReturnConfigTimeslot.Direction = ParameterDirection.Output;

            CheckTimeslotBySubtypeModel Result = new CheckTimeslotBySubtypeModel();

            try
            {
                List<ReturnConfigTimeslotModel> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_QUERY_CONFIG_TIMESLOT.CHECK_TIMESLOT_BY_SUBTYPE",
                     new
                     {
                         PartnerSubtype = query.partnersubtype.ToSafeString(),
                         AccessMode = query.accessmode.ToSafeString(),
                         // out
                         ReturnCode = ReturnCode,
                         ReturnMessage = ReturnMessage,
                         ReturnConfigTimeslot = ReturnConfigTimeslot
                     }).ToList();

                Result.ReturnCode = (ReturnCode.Value == null || ReturnCode.Value.ToSafeString() == "null") ? "" : ReturnCode.Value.ToSafeString();
                Result.ReturnMessage = (ReturnMessage.Value == null || ReturnMessage.Value.ToSafeString() == "null") ? "" : ReturnMessage.Value.ToSafeString();
                Result.ReturnConfigTimeslot = executeResult;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, Result, log, "Success", "", "");

            }
            catch (Exception ex)
            {
                Result.ReturnCode = "-1";
                Result.ReturnMessage = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", "inner" + ex.InnerException + " Message:" + ex.Message, "");
            }

            return Result;
        }

    }
}
