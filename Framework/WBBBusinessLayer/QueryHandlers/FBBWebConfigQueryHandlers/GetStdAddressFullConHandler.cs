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
    class GetStdAddressFullConHandler : IQueryHandler<STDFullConQuery, StdAddressFullConListResult>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<StdAddressFullConList> _objService;

        public GetStdAddressFullConHandler(ILogger logger, IEntityRepository<StdAddressFullConList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public StdAddressFullConListResult Handle(STDFullConQuery query)
        {

            StdAddressFullConListResult result = new StdAddressFullConListResult();

            try
            {
                _logger.Info("GetStdAddressFullConHandler : Start.");
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.InputOutput;
                ret_code.Value = query.Return_Code;

                var ret_msgerr = new OracleParameter();
                ret_msgerr.OracleDbType = OracleDbType.Varchar2;
                ret_msgerr.Size = 2000;
                ret_msgerr.Direction = ParameterDirection.Output;

                var cur = new OracleParameter();
                cur.ParameterName = "cur";
                cur.OracleDbType = OracleDbType.RefCursor;
                cur.Direction = ParameterDirection.Output;

                List<StdAddressFullConList> data = new List<StdAddressFullConList>();
                data = _objService.ExecuteReadStoredProc("WBB.PKG_FBBPAYG_NETWORK_FULL_CONN.p_get_full_conn",
                            new
                            {
                                //p_olt_no = query.OltNo,
                                //p_region = query.Region,
                                //p_cre_dt_from = "",DateTime.Today.Date.ToString("dd/MM/yyyy"),
                                //p_cre_dt_to = "",DateTime.Today.Date.ToString("dd/MM/yyyy"),

                                //  return code
                                ret_code = ret_code,
                                ret_msgerr = ret_msgerr,
                                cur = cur

                            }).ToList();
                result.Return_Code = ret_code.Value != null ? Convert.ToInt32(ret_code.Value.ToString()) : -1;
                result.Return_Desc = ret_msgerr.Value.ToString();
                result.Data = data;
                return result;

            }
            catch (Exception ex)
            {
                _logger.Info("GetStdAddressFullConHandler : Error.");
                _logger.Info(ex.Message);

                result.Return_Code = -1;
                result.Return_Desc = "Error call save event service " + ex.Message;
                return result;
            }

        }

    }
}
