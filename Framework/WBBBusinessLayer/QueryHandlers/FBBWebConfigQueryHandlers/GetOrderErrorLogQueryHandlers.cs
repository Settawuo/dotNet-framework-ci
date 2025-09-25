using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class GetOrderErrorLogQueryHandlers : IQueryHandler<GetOrderErrorLogQuery, OrderErrorLogModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<OrderErrorLogList> _objService;

        public GetOrderErrorLogQueryHandlers(ILogger logger, IEntityRepository<OrderErrorLogList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public OrderErrorLogModel Handle(GetOrderErrorLogQuery query)
        {
            OrderErrorLogModel orderErrorLog = new OrderErrorLogModel();

            try
            {
                var p_page_count = new OracleParameter();
                p_page_count.ParameterName = "P_PAGE_COUNT";
                p_page_count.OracleDbType = OracleDbType.Decimal;
                p_page_count.Direction = ParameterDirection.Output;

                var p_return_code = new OracleParameter();
                p_return_code.ParameterName = "P_RETURN_CODE";
                p_return_code.Size = 2000;
                p_return_code.OracleDbType = OracleDbType.Varchar2;
                p_return_code.Direction = ParameterDirection.Output;

                var p_return_message = new OracleParameter();
                p_return_message.ParameterName = "P_RETURN_MESSAGE";
                p_return_message.Size = 2000;
                p_return_message.OracleDbType = OracleDbType.Varchar2;
                p_return_message.Direction = ParameterDirection.Output;

                var p_res_data = new OracleParameter();
                p_res_data.ParameterName = "P_RES_DATA";
                p_res_data.OracleDbType = OracleDbType.RefCursor;
                p_res_data.Direction = ParameterDirection.Output;

                _logger.Info("StartPKG_FBB_RESEND_ORDER");

                List<OrderErrorLogList> executeResult = _objService.ExecuteReadStoredProc("PKG_FBB_RESEND_ORDER.PROC_RESEND_ORDER",
                            new
                            {
                                P_DATE_FROM = query.P_DATE_FROM,
                                P_DATE_TO = query.P_DATE_TO,
                                P_ID_CARD_NO = query.P_ID_CARD_NO,
                                P_REQUEST_STATUS = query.P_REQUEST_STATUS,
                                P_PAGE_INDEX = query.P_PAGE_INDEX,
                                P_PAGE_SIZE = query.P_PAGE_SIZE,


                                //  p_return_code code
                                p_page_count = p_page_count,
                                p_return_code = p_return_code,
                                p_return_message = p_return_message,
                                p_res_data = p_res_data

                            }).ToList();

                orderErrorLog.P_PAGE_COUNT = Int32.Parse(p_page_count.Value.ToString());
                orderErrorLog.P_RETURN_CODE = p_return_code.Value != null ? p_return_code.Value.ToString() : "-1";
                orderErrorLog.P_RETURN_MESSAGE = p_return_message.Value.ToString();
                orderErrorLog.P_RES_DATA = executeResult;

                _logger.Info("EndPKG_FBB_RESEND_ORDER" + p_return_message.Value.ToString());


                return orderErrorLog;

            }
            catch (Exception ex)
            {
                _logger.Info("Error call PKG_FBB_RESEND_ORDER handles : " + ex.Message);
                orderErrorLog.P_RETURN_CODE = "-1";
                orderErrorLog.P_RETURN_MESSAGE = ex.Message;
                return null;
            }

        }
    }


    public class GetLogStatusQueryHandler : IQueryHandler<GetStatusLogQuery, List<StatusLogDropdown>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _interfaceLog;

        public GetLogStatusQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> interfaceLog)
        {
            _logger = logger;
            _interfaceLog = interfaceLog;
        }

        public List<StatusLogDropdown> Handle(GetStatusLogQuery query)
        {
            List<string> status = new List<string> { "0", "-1", "Success" };
            List<string> status_n = new List<string> { "PENDING", "ERROR" };
            var a = (from l in _interfaceLog.Get()
                     where status_n.Contains(l.REQUEST_STATUS)
                     //where !status.Contains(l.REQUEST_STATUS)s
                     //&& !l.REQUEST_STATUS.Contains(@"rowCount")
                     select new StatusLogDropdown
                     {
                         TEXT = l.REQUEST_STATUS,
                         VALUE = l.REQUEST_STATUS
                     }).Distinct().ToList();

            return a;
        }
    }
}

