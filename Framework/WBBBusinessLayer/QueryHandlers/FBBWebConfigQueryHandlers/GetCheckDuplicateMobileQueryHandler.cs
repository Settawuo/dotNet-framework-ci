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
    public class GetCheckDuplicateMobileQueryHandler : IQueryHandler<GetCheckDuplicateMobileQuery, List<DuplicateMobileList>>
    {
        private readonly ILogger _logger;
        private readonly IAirNetEntityRepository<DuplicateMobileList> _objService;

        public GetCheckDuplicateMobileQueryHandler(ILogger logger, IAirNetEntityRepository<DuplicateMobileList> objService)
        {
            _logger = logger;
            _objService = objService;
        }

        public List<DuplicateMobileList> Handle(GetCheckDuplicateMobileQuery command)
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

                List<DuplicateMobileList> executeResult = _objService.ExecuteReadStoredProc("AIR_ADMIN.PKG_FBB_RPTPORT006.CHECK_DUPLICATE_MOBILE",
                            new
                            {
                                date_from = command.dateFrom,
                                date_to = command.dateTo,

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
