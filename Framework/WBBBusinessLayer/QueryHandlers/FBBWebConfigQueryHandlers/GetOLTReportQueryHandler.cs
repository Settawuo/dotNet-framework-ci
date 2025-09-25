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
    class GetOLTReportQueryHandler : IQueryHandler<OLTQuery, List<OLTList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<OLTList> _objService;

        public GetOLTReportQueryHandler(ILogger logger, IEntityRepository<OLTList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<OLTList> Handle(OLTQuery query)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;


                var cur = new OracleParameter();
                cur.ParameterName = "cur";
                cur.OracleDbType = OracleDbType.RefCursor;
                cur.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                List<OLTList> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBPAYG_REPORT_OLT.p_get_report_olt",
                            new
                            {
                                p_inv_dt_from = query.dateFrom,
                                p_inv_dt_to = query.dateTo,

                                //  return code
                                ret_code = ret_code,
                                cur = cur,
                                ret_msg = ret_msg

                            }).ToList();

                query.Return_Code = 1;
                query.Return_Desc = "Success";

                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                query.Return_Code = -1;
                query.Return_Desc = "Error call save event service " + ex.Message;

                return null;
            }

        }

    }
}
