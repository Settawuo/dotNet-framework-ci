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
    class GetOSPReportQueryHandler : IQueryHandler<OSPQuery, List<OSPList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<OSPList> _objService;

        public GetOSPReportQueryHandler(ILogger logger, IEntityRepository<OSPList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<OSPList> Handle(OSPQuery query)
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

                List<OSPList> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBPAYG_REPORT_OSP.p_get_report_osp",
                            new
                            {
                                p_inv_dt_from = query.dateFrom,
                                p_inv_dt_to = query.dateTo,

                                //  return code
                                ret_code = ret_code,
                                cur = cur

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
