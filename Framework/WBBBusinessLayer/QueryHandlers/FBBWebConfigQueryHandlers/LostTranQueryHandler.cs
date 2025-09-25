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
    public class LostTranQueryHandler : IQueryHandler<LostTranQuery, List<LosttranModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<LosttranModel> _objService;

        public LostTranQueryHandler(ILogger logger, IEntityRepository<LosttranModel> objService)
        {
            _logger = logger;
            _objService = objService;
        }
        public List<LosttranModel> Handle(LostTranQuery query)
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
                var cur_losttran_log = new OracleParameter
                {
                    ParameterName = "cur_losttran_log",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };


                _logger.Info("Start ResendFoa Query Handler Call  : ");

                List<LosttranModel> executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_FOA_ORDER_MANAGEMENT.p_get_lostTran_log",
                new
                {
                    query.Acess_no,
                    query.Date,
                    cur_losttran_log,
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
