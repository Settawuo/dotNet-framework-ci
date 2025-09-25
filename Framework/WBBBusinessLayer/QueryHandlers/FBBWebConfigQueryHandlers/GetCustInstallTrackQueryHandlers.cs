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
    public class GetCustInstallTrackQueryHandlers : IQueryHandler<GetCusInstallTrackQuery, List<CusInstallTrackList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<CusInstallTrackList> _objService;

        public GetCustInstallTrackQueryHandlers(ILogger logger, IEntityRepository<CusInstallTrackList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<CusInstallTrackList> Handle(GetCusInstallTrackQuery command)
        {
            try
            {
                var ret_code = new OracleParameter();
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var res_data = new OracleParameter();
                res_data.ParameterName = "res_data";
                res_data.OracleDbType = OracleDbType.RefCursor;
                res_data.Direction = ParameterDirection.Output;

                List<CusInstallTrackList> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_fbbdorm_payg_RPT001.rpt_cust_install",
                            new
                            {
                                date_from = (command.dateFrom != null) ? command.dateFrom.Value.Date.ToString("dd/MM/yyyy") : "",
                                date_to = (command.dateTo != null) ? command.dateTo.Value.Date.ToString("dd/MM/yyyy") : "",

                                //  return code
                                ret_code = ret_code,
                                ret_msg = ret_msg,
                                res_data = res_data

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
