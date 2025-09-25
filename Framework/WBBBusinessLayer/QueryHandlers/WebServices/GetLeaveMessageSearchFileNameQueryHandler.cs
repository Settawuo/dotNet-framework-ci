using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetLeaveMessageSearchFileNameQueryHandler : IQueryHandler<GetLeaveMessageSearchFileNameQuery, List<SearchLeaveMsgFileNameList>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<SearchLeaveMsgFileNameList> _objService;

        public GetLeaveMessageSearchFileNameQueryHandler(ILogger logger, IWBBUnitOfWork uow
            , IEntityRepository<SearchLeaveMsgFileNameList> objService)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
        }

        public List<SearchLeaveMsgFileNameList> Handle(GetLeaveMessageSearchFileNameQuery query)
        {
            try
            {
                _logger.Info("GetLeaveMessageSearchFileNameQueryHandler Start");
                var return_code = new OracleParameter();
                return_code.ParameterName = "return_code";
                return_code.OracleDbType = OracleDbType.Decimal;
                return_code.Direction = ParameterDirection.Output;

                var return_message = new OracleParameter();
                return_message.ParameterName = "return_message";
                return_message.OracleDbType = OracleDbType.Varchar2;
                return_message.Size = 2000;
                return_message.Direction = ParameterDirection.Output;

                var p_seacrh_file_name = new OracleParameter();
                p_seacrh_file_name.ParameterName = "p_seacrh_file_name";
                p_seacrh_file_name.OracleDbType = OracleDbType.RefCursor;
                p_seacrh_file_name.Direction = ParameterDirection.Output;

                var outp = new List<object>();
                var paramOut = outp.ToArray();

                List<SearchLeaveMsgFileNameList> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBOR021.PROC_REPORT_FILE_NAME",
                    new
                    {
                        p_username = query.P_USERNAME,
                        p_file_name = query.P_FILE_NAME,
                        p_start_date = query.P_START_DATE,
                        p_end_date = query.P_END_DATE,

                        // return code
                        return_code = return_code,
                        return_message = return_message,
                        p_seacrh_file_name = p_seacrh_file_name

                    }).ToList();

                if (return_code.Value.ToSafeString() == "0") // return 0 pass value to screen 
                {
                    _logger.Info("End WBB.PKG_FBBOR021.PROC_REPORT_FILE_NAME output msg: " + query.return_message);
                    return executeResult;

                }
                else //return -1 error
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBBOR021.PROC_REPORT_FILE_NAME output msg: " + return_message);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBBOR021.PROC_REPORT_FILE_NAME" + ex.Message);
                return null;
            }

        }
    }
}
