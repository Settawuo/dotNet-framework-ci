using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{

    public class TrackingCompetitorRptQueryHandler : IQueryHandler<TrackingCompetitorRptQuery, List<TrackingCompetitorRptList>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<TrackingCompetitorRptList> _objService;

        public TrackingCompetitorRptQueryHandler(ILogger logger, IAirNetEntityRepository<TrackingCompetitorRptList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<TrackingCompetitorRptList> Handle(TrackingCompetitorRptQuery query)
        {
            try
            {
                _logger.Info("TrackingCompetitorRptQueryHandler Start");
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var ret_data = new OracleParameter();
                ret_data.ParameterName = "ret_data";
                ret_data.OracleDbType = OracleDbType.RefCursor;
                ret_data.Direction = ParameterDirection.Output;

                List<TrackingCompetitorRptList> executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_FBB_RPTPORT007.TRACKING_COMPETITOR_REPORT",

                            new
                            {
                                order_date_from = query.order_date_from,
                                order_date_to = query.order_date_to,
                                order_status = query.order_status,

                                //  return code
                                ret_code = ret_code,
                                ret_msg = ret_msg,
                                ret_data = ret_data

                            }).ToList();

                query.ret_code = 1;
                query.ret_msg = "Success";
                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service AIR_ADMIN.PKG_FBB_RPTPORT007.TRACKING_COMPETITOR_REPORT" + ex.Message);
                query.ret_code = -1;
                query.ret_msg = "Error";

                return null;
            }

        }
    }
}

