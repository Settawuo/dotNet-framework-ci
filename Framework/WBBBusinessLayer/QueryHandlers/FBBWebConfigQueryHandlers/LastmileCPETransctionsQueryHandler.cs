using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class LastmileCPETransctionsQueryHandler : IQueryHandler<LastmileAndCPETransactionsQuery, List<LastmileAndCPEReportList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<LastmileAndCPEReportList> _objService;

        public LastmileCPETransctionsQueryHandler(ILogger logger, IEntityRepository<LastmileAndCPEReportList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<LastmileAndCPEReportList> Handle(LastmileAndCPETransactionsQuery command)
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

                List<LastmileAndCPEReportList> executeResult = _objService.ExecuteReadStoredProc("WBB.pkg_fbbpayg_sumlastmile.p_get_summary_lastmile_and_cpe",
                            new
                            {
                                p_vendor = command.oltbrand.ToSafeString(),
                                p_phase = command.phase.ToSafeString(),
                                p_region = command.region.ToSafeString(),
                                p_inv_dt_from = command.dateFrom.ToSafeString(),
                                p_inv_dt_to = command.dateTo.ToSafeString(),
                                p_product_name = command.product.ToSafeString(),
                                p_addrss_id = command.addressid.ToSafeString(),

                                //  return code
                                ret_code = ret_code,
                                cur = cur

                            }).ToList();

                command.Return_Code = 1;
                command.Return_Desc = "Success";
                return executeResult;

            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);

                command.Return_Code = -1;
                command.Return_Desc = "Error call save event service " + ex.Message;

                return null;
            }

        }

    }
}
