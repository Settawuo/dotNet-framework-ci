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
    public class ResendWSSAPS4QueryHandler : IQueryHandler<ResendSsaps4Query, List<ResendWssaps4Model>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ResendWssaps4Model> _objService;

        public ResendWSSAPS4QueryHandler(ILogger logger, IEntityRepository<ResendWssaps4Model> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public List<ResendWssaps4Model> Handle(ResendSsaps4Query query)
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

                
                var cur_resend_ws_saps4 = new OracleParameter
                {
                    ParameterName = "cur_resend_ws_saps4",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };


                _logger.Info("Start ResendFoa Query Handler Call  : PKG_FBBPAYG_FOA_RESEND_ORDER ");
               // List<LosttranModel> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_get_lostTran_log",

              List <ResendWssaps4Model> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBBPAYG_FOA_RESEND_ORDER.p_get_error_log_saps4",
                new
                {
                    
                    query.p_date_start,
                    query.p_date_to,

                    cur_resend_ws_saps4 ,
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
