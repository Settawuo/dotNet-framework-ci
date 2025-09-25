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
    class AppointmentDetailQueryHandler : IQueryHandler<AppointmentDetailQuery, List<AppointmentDetailModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<AppointmentDetailModel> _objService;

        public AppointmentDetailQueryHandler(ILogger logger, IEntityRepository<AppointmentDetailModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<AppointmentDetailModel> Handle(AppointmentDetailQuery query)
        {

            try
            {
                _logger.Info("AppointmentDetailQueryHandler : Start.");
                //var ret_code = new OracleParameter();
                //ret_code.OracleDbType = OracleDbType.Decimal;
                //ret_code.Direction = ParameterDirection.InputOutput;
                //ret_code.Value = query.Return_Code;

                //var p_service = new OracleParameter();
                //p_service.OracleDbType = OracleDbType.Varchar2;
                //p_service.Size = 2000;
                //p_service.Direction = ParameterDirection.Input;
                //p_service.Value = query.Service;

                //var p_ord_dt_from = new OracleParameter();
                //p_ord_dt_from.OracleDbType = OracleDbType.Varchar2;
                //p_ord_dt_from.Size = 2000;
                //p_ord_dt_from.Direction = ParameterDirection.Input;
                //p_ord_dt_from.Value = query.DateFrom;

                //var p_ord_dt_to = new OracleParameter();
                //p_ord_dt_to.OracleDbType = OracleDbType.Varchar2;
                //p_ord_dt_to.Size = 2000;
                //p_ord_dt_to.Direction = ParameterDirection.Input;
                //p_ord_dt_to.Value = query.DateTo;

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

                List<AppointmentDetailModel> data = new List<AppointmentDetailModel>();
                data = _objService.ExecuteReadStoredProc("WBB.pkg_fbss_activity_work_order.p_get_work_order_dtl",
                            new
                            {
                                p_ord_dt_from = query.dateFrom.ToString(),
                                p_ord_dt_to = query.dateTo.ToString(),
                                p_service = query.service.ToString(),

                                ret_code = ret_code,
                                ret_msg = ret_msg,
                                cur = cur

                            }).ToList();
                //result.Return_Code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToString()) : -1;
                //result.Return_Desc = ret_msgerr.Value.ToString();
                //result.Data = data;
                return data;

            }
            catch (Exception ex)
            {
                _logger.Info("AppointmentDetailQueryHandler : Error.");
                _logger.Info(ex.Message);

                //result.Return_Code = -1;
                //result.Return_Desc = "Error call service " + ex.Message;
                return new List<AppointmentDetailModel>();
            }

        }
    }
}
