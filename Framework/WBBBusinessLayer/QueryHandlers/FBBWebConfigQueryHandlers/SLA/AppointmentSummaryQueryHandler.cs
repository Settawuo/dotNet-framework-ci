using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SLA
{
    class AppointmentSummaryQueryHandler : IQueryHandler<AppointmentSummaryQuery, List<AppointmentSummaryModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<AppointmentSummaryModel> _objService;

        public AppointmentSummaryQueryHandler(ILogger logger, IEntityRepository<AppointmentSummaryModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<AppointmentSummaryModel> Handle(AppointmentSummaryQuery query)
        {

            try
            {
                _logger.Info("AppointmentSummaryQueryHandler : Start.");

                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var cur = new OracleParameter();
                cur.OracleDbType = OracleDbType.RefCursor;
                cur.Direction = ParameterDirection.Output;

                List<AppointmentSummaryModel> data = new List<AppointmentSummaryModel>();
                data = _objService.ExecuteReadStoredProc("WBB.PKG_FBSS_ACTIVITY_WORK_ORDER.p_get_work_order_summary",
                            new
                            {
                                p_ord_dt_from = query.dateFrom,
                                p_ord_dt_to = query.dateTo,
                                p_service = query.service,
                                ret_code = ret_code,
                                ret_msg = ret_msg,
                                cur = cur

                            }).ToList();
                return data;

            }
            catch (Exception ex)
            {
                _logger.Info("AppointmentSummaryQueryHandler : Error.");
                _logger.Info(ex.Message);

                //result.Return_Code = -1;
                //result.Return_Desc = "Error call service " + ex.Message;
                return new List<AppointmentSummaryModel>(); ;
            }

        }

    }
}
