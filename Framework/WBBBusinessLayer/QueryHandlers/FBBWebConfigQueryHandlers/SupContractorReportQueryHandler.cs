using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SupContractorReportQueryHandler : IQueryHandler<SupContractorReportQuery, List<SupContractorReportList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<SupContractorReportList> _objService;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;
        private readonly IWBBUnitOfWork _uow;

        public SupContractorReportQueryHandler(ILogger logger, IEntityRepository<SupContractorReportList> objService, IEntityRepository<FBB_HISTORY_LOG> historyLog, IWBBUnitOfWork uow)
        {
            _logger = logger;
            _objService = objService;
            _historyLog = historyLog;
            _uow = uow;
        }

        public List<SupContractorReportList> Handle(SupContractorReportQuery command)
        {
            var historyLog = new FBB_HISTORY_LOG();
            string _PARAMS = " ORG_ID : " + command.ORG_ID.ToSafeString();
            _PARAMS += " SUB_CONTRACTOR_NAME_TH : " + command.SUB_CONTRACTOR_NAME_TH.ToSafeString();
            _PARAMS += " STORAGE_LOCATION :" + command.STORAGE_LOCATION.ToSafeString();
            _PARAMS += " PHASE : " + command.PHASE.ToSafeString();
            _PARAMS += " REQUEST_BY : " + command.REQUEST_BY.ToSafeString();
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;


                var cur = new OracleParameter();
                cur.ParameterName = "cur";
                cur.OracleDbType = OracleDbType.RefCursor;
                cur.Direction = ParameterDirection.Output;

                var p_org_id = new OracleParameter();
                p_org_id.ParameterName = "p_org_id";
                p_org_id.OracleDbType = OracleDbType.Varchar2;
                p_org_id.Direction = ParameterDirection.Input;
                p_org_id.Value = command.ORG_ID.ToSafeString();

                var p_sub_contract_name_th = new OracleParameter();
                p_sub_contract_name_th.ParameterName = "p_sub_contract_name_th";
                p_sub_contract_name_th.OracleDbType = OracleDbType.Varchar2;
                p_sub_contract_name_th.Direction = ParameterDirection.Input;
                p_sub_contract_name_th.Value = command.SUB_CONTRACTOR_NAME_TH.ToSafeString();

                var p_storage_location = new OracleParameter();
                p_storage_location.ParameterName = "p_storage_location";
                p_storage_location.OracleDbType = OracleDbType.Varchar2;
                p_storage_location.Direction = ParameterDirection.Input;
                p_storage_location.Value = command.STORAGE_LOCATION.ToSafeString();

                var p_phase = new OracleParameter();
                p_phase.ParameterName = "p_phase";
                p_phase.OracleDbType = OracleDbType.Varchar2;
                p_phase.Direction = ParameterDirection.Input;
                p_phase.Value = command.PHASE.ToSafeString();

                var p_request_by = new OracleParameter();
                p_phase.ParameterName = "p_request_by";
                p_phase.OracleDbType = OracleDbType.Varchar2;
                p_phase.Direction = ParameterDirection.Input;
                p_phase.Value = command.REQUEST_BY.ToSafeString();

                List<SupContractorReportList> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_REPORT_SUBCONTRACT.P_GET_REPORT_SUBCONTRACT",
                              new
                              {
                                  p_org_id = command.ORG_ID.ToSafeString(),
                                  p_sub_contract_name_th = command.SUB_CONTRACTOR_NAME_TH.ToSafeString(),
                                  p_storage_location = command.STORAGE_LOCATION.ToSafeString(),
                                  p_phase = command.PHASE.ToSafeString(),
                                  p_request_by = command.REQUEST_BY.ToSafeString(),

                                  cur = cur,
                                  ret_code = ret_code,
                                  ret_msg = ret_msg,

                              }).ToList();
                return executeResult;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                historyLog.ACTION = ActionHistory.NONE.ToString();
                historyLog.APPLICATION = "SupContractor";
                historyLog.DESCRIPTION = "Error Message : [" + ex.Message.ToSafeString() + "] InnerException : [" + ex.InnerException.ToSafeString() + "] Params : { " + _PARAMS + " }";
                historyLog.REF_KEY = "Service : SupContractorReportQueryHandler";
                historyLog.REF_NAME = "SupContractorReportQuery";
                historyLog.CREATED_BY = command.REQUEST_BY.ToSafeString();
                historyLog.CREATED_DATE = DateTime.Now;
                _historyLog.Create(historyLog);
                _uow.Persist();
                return null;
            }
        }
    }
}
