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
    public class CheckPremiumFlagHandler : IQueryHandler<CheckPremiumFlagDataQuery, CheckPremiumFlagModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ReturnPremiumTimeSlotModel> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public CheckPremiumFlagHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<ReturnPremiumTimeSlotModel> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
        }

        public CheckPremiumFlagModel Handle(CheckPremiumFlagDataQuery query)
        {
            InterfaceLogCommand log = null;
            DateTime Curr_DateTime = DateTime.Now;
            log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionID, "CheckPremiumFlag", "CheckPremiumFlagHandler", null, "FBB", "");

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

            var SubAccessMode = new OracleParameter();
            SubAccessMode.ParameterName = "SubAccessMode";
            SubAccessMode.OracleDbType = OracleDbType.Varchar2;
            SubAccessMode.Size = 2000;
            SubAccessMode.Direction = ParameterDirection.Output;

            var RecurringCharges = new OracleParameter();
            RecurringCharges.ParameterName = "RecurringCharges";
            RecurringCharges.OracleDbType = OracleDbType.Varchar2;
            RecurringCharges.Size = 2000;
            RecurringCharges.Direction = ParameterDirection.Output;

            var LocationCodes = new OracleParameter();
            LocationCodes.ParameterName = "LocationCodes";
            LocationCodes.OracleDbType = OracleDbType.Varchar2;
            LocationCodes.Size = 2000;
            LocationCodes.Direction = ParameterDirection.Output;

            var AccessModes = new OracleParameter();
            AccessModes.ParameterName = "AccessModes";
            AccessModes.OracleDbType = OracleDbType.Varchar2;
            AccessModes.Size = 2000;
            AccessModes.Direction = ParameterDirection.Output;

            var ReturnPremiumTimeSlot = new OracleParameter();
            ReturnPremiumTimeSlot.OracleDbType = OracleDbType.RefCursor;
            ReturnPremiumTimeSlot.Direction = ParameterDirection.Output;

            CheckPremiumFlagModel Result = new CheckPremiumFlagModel();

            try
            {
                List<ReturnPremiumTimeSlotModel> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_QUERY_CONFIG_TIMESLOT.CHECK_PREMIUM_FLAG",
                     new
                     {
                         RecurringCharge = query.RecurringCharge.ToSafeString(),
                         LocationCode = query.LocationCode.ToSafeString(),
                         AccessMode = query.AccessMode.ToSafeString(),
                         PartnerSubtype = query.PartnerSubtype.ToSafeString(),
                         MemoFlag = query.MemoFlag.ToSafeString(),
                         // out
                         ReturnCode = ReturnCode,
                         ReturnMessage = ReturnMessage,
                         SubAccessMode = SubAccessMode,
                         RecurringCharges = RecurringCharges,
                         LocationCodes = LocationCodes,
                         AccessModes = AccessModes,
                         ReturnPremiumTimeSlot = ReturnPremiumTimeSlot
                     }).ToList();

                Result.ReturnCode = (ReturnCode.Value == null || ReturnCode.Value.ToSafeString() == "null") ? "" : ReturnCode.Value.ToSafeString();
                Result.ReturnMessage = (ReturnMessage.Value == null || ReturnMessage.Value.ToSafeString() == "null") ? "" : ReturnMessage.Value.ToSafeString();
                Result.SubAccessMode = (SubAccessMode.Value == null || SubAccessMode.Value.ToSafeString() == "null") ? "" : SubAccessMode.Value.ToSafeString();
                Result.RecurringCharges = (RecurringCharges.Value == null || RecurringCharges.Value.ToSafeString() == "null") ? "" : RecurringCharges.Value.ToSafeString();
                Result.LocationCodes = (LocationCodes.Value == null || LocationCodes.Value.ToSafeString() == "null") ? "" : LocationCodes.Value.ToSafeString();
                Result.AccessModes = (AccessModes.Value == null || AccessModes.Value.ToSafeString() == "null") ? "" : AccessModes.Value.ToSafeString();
                Result.ReturnPremiumTimeSlot = executeResult;
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
