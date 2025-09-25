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
    public class ResendFoaQueryHandler : IQueryHandler<ResendFoaQuery, List<ResendFoaModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ResendFoaModel> _objService;

        public ResendFoaQueryHandler(ILogger logger, IEntityRepository<ResendFoaModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public List<ResendFoaModel> Handle(ResendFoaQuery query)
        {
            try
            {
                var ret_code = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    Direction = ParameterDirection.Output
                };
                var ret_msg = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = ParameterDirection.Output
                };
                var cur_resend_error_log = new OracleParameter
                {
                    ParameterName = "cur_resend_error_log",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };


                _logger.Info("Start ResendFoa Query Handler Call  : ");

                List<ResendFoaModel> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_get_foa_resend_error_log",
                new
                {
                    query.Acess_no,
                    query.flag,
                    cur_resend_error_log,
                    ret_code = ret_code,
                    ret_msg = ret_msg,

                }).ToList();

                _logger.Info("End ResendFoa Query Handler Call  : ");

                return executeResult;

            }
            catch (Exception ex)
            {

                _logger.Info("Error ResendFoaQueryHandler Call  : " + ex.Message);
                return null;
            }


        }
    }
}
